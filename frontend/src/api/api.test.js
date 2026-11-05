import { API_URL, fetchWithAuth, getHeaders } from "./api";
import { lastFetchCall, mockFetchResponse } from "../testUtils/mockFetch";

describe("api", () => {
	const originalLocation = window.location;

	beforeEach(() => {
		// jsdom não implementa navegação; substituímos location para observar o redirect.
		delete window.location;
		window.location = { href: "" };
	});

	afterAll(() => {
		window.location = originalLocation;
	});

	describe("getHeaders", () => {
		it("não envia Authorization quando não há token", () => {
			expect(getHeaders()).toEqual({ "Content-Type": "application/json" });
		});

		it("envia o token como Bearer quando existe", () => {
			localStorage.setItem("token", "abc");

			expect(getHeaders().Authorization).toBe("Bearer abc");
		});
	});

	describe("fetchWithAuth", () => {
		it("monta a URL com API_URL e envia headers de autenticação", async () => {
			localStorage.setItem("token", "abc");
			mockFetchResponse();

			await fetchWithAuth("/categories", { method: "POST", headers: { "X-Extra": "1" } });

			const { url, options } = lastFetchCall();
			expect(url).toBe(`${API_URL}/categories`);
			expect(options.method).toBe("POST");
			expect(options.headers).toEqual({
				"Content-Type": "application/json",
				Authorization: "Bearer abc",
				"X-Extra": "1"
			});
		});

		it("retorna a resposta do fetch", async () => {
			const response = mockFetchResponse({ status: 200 });

			await expect(fetchWithAuth("/categories")).resolves.toBe(response);
		});

		it("em 401 remove o token e redireciona para /login", async () => {
			localStorage.setItem("token", "expirado");
			mockFetchResponse({ ok: false, status: 401 });

			await fetchWithAuth("/categories");

			expect(localStorage.getItem("token")).toBeNull();
			expect(window.location.href).toBe("/login");
		});

		it("em 401 no endpoint de login não redireciona", async () => {
			localStorage.setItem("token", "abc");
			mockFetchResponse({ ok: false, status: 401 });

			await fetchWithAuth("/auth/login");

			expect(localStorage.getItem("token")).toBe("abc");
			expect(window.location.href).toBe("");
		});
	});
});
