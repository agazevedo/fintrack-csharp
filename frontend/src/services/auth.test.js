import { API_URL } from "../api/api";
import { getToken, isAuthenticated, login, logout } from "./auth";
import { lastFetchCall, mockFetchResponse } from "../testUtils/mockFetch";

describe("auth service", () => {
	describe("login", () => {
		it("faz POST em /auth/login com as credenciais", async () => {
			mockFetchResponse({ json: { token: "abc" } });

			await login("alice", "secret123");

			const { url, options } = lastFetchCall();
			expect(url).toBe(`${API_URL}/auth/login`);
			expect(options.method).toBe("POST");
			expect(JSON.parse(options.body)).toEqual({ username: "alice", password: "secret123" });
		});

		it("salva o token e retorna true quando as credenciais são válidas", async () => {
			mockFetchResponse({ json: { token: "abc" } });

			await expect(login("alice", "secret123")).resolves.toBe(true);
			expect(localStorage.getItem("token")).toBe("abc");
		});

		it("retorna false e não salva token quando a API recusa", async () => {
			mockFetchResponse({ ok: false, status: 401 });

			await expect(login("alice", "errada")).resolves.toBe(false);
			expect(localStorage.getItem("token")).toBeNull();
		});

		it("retorna false quando ocorre erro de rede", async () => {
			jest.spyOn(console, "error").mockImplementation(() => {});
			global.fetch.mockRejectedValueOnce(new Error("Network error"));

			await expect(login("alice", "secret123")).resolves.toBe(false);
		});
	});

	it("getToken e isAuthenticated refletem o token salvo", () => {
		expect(isAuthenticated()).toBe(false);
		expect(getToken()).toBeNull();

		localStorage.setItem("token", "abc");

		expect(isAuthenticated()).toBe(true);
		expect(getToken()).toBe("abc");
	});

	it("logout remove o token", () => {
		localStorage.setItem("token", "abc");

		logout();

		expect(isAuthenticated()).toBe(false);
	});
});
