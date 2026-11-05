import { API_URL } from "../api/api";
import { lastFetchCall, mockFetchResponse } from "./mockFetch";

/**
 * Suíte compartilhada pelos services de CRUD (categorias, itens, despesas),
 * que seguem o mesmo contrato: get / create / update / delete.
 */
export function describeCrudService({ name, endpoint, service, payload, messages }) {
	const { getAll, create, update, remove } = service;

	describe(name, () => {
		beforeEach(() => {
			localStorage.setItem("token", "fake-token");
		});

		describe("getAll", () => {
			it(`faz GET em ${endpoint} com token e retorna o JSON`, async () => {
				mockFetchResponse({ json: [{ id: 1 }] });

				await expect(getAll()).resolves.toEqual([{ id: 1 }]);

				const { url, options } = lastFetchCall();
				expect(url).toBe(`${API_URL}${endpoint}`);
				expect(options.method).toBeUndefined();
				expect(options.headers.Authorization).toBe("Bearer fake-token");
			});

			it("lança erro quando a resposta não é ok", async () => {
				mockFetchResponse({ ok: false, status: 500 });

				await expect(getAll()).rejects.toThrow(messages.getAll);
			});
		});

		describe("create", () => {
			it(`faz POST em ${endpoint} com o body serializado`, async () => {
				mockFetchResponse({ status: 201, json: { id: 10, ...payload } });

				await expect(create(payload)).resolves.toEqual({ id: 10, ...payload });

				const { url, options } = lastFetchCall();
				expect(url).toBe(`${API_URL}${endpoint}`);
				expect(options.method).toBe("POST");
				expect(JSON.parse(options.body)).toEqual(payload);
				expect(options.headers["Content-Type"]).toBe("application/json");
			});

			it("lança erro com a mensagem retornada pela API", async () => {
				mockFetchResponse({ ok: false, status: 400, json: { message: "Dados inválidos" } });

				await expect(create(payload)).rejects.toThrow("Dados inválidos");
			});
		});

		describe("update", () => {
			it(`faz PUT em ${endpoint}/:id com o body serializado`, async () => {
				mockFetchResponse({ json: { id: 5, ...payload } });

				await expect(update(5, payload)).resolves.toEqual({ id: 5, ...payload });

				const { url, options } = lastFetchCall();
				expect(url).toBe(`${API_URL}${endpoint}/5`);
				expect(options.method).toBe("PUT");
				expect(JSON.parse(options.body)).toEqual(payload);
			});

			it("lança erro com o texto da resposta", async () => {
				mockFetchResponse({ ok: false, status: 400, text: "Falha de validação" });

				await expect(update(5, payload)).rejects.toThrow("Falha de validação");
			});

			it("usa mensagem padrão quando a resposta não tem texto", async () => {
				mockFetchResponse({ ok: false, status: 404, text: "" });

				await expect(update(5, payload)).rejects.toThrow(messages.update);
			});
		});

		describe("delete", () => {
			it(`faz DELETE em ${endpoint}/:id e retorna true`, async () => {
				mockFetchResponse({ status: 204 });

				await expect(remove(7)).resolves.toBe(true);

				const { url, options } = lastFetchCall();
				expect(url).toBe(`${API_URL}${endpoint}/7`);
				expect(options.method).toBe("DELETE");
			});

			it("lança erro quando a resposta não é ok", async () => {
				mockFetchResponse({ ok: false, status: 404 });

				await expect(remove(7)).rejects.toThrow(messages.delete);
			});
		});
	});
}
