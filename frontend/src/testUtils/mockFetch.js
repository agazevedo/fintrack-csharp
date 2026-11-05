// Enfileira uma resposta para a próxima chamada de fetch.
export function mockFetchResponse({ ok = true, status = 200, json = {}, text = "" } = {}) {
	const response = {
		ok,
		status,
		json: jest.fn().mockResolvedValue(json),
		text: jest.fn().mockResolvedValue(text)
	};

	global.fetch.mockResolvedValueOnce(response);

	return response;
}

export function lastFetchCall() {
	const [url, options] = global.fetch.mock.calls[global.fetch.mock.calls.length - 1];
	return { url, options };
}
