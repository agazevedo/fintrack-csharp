// Executado pelo Jest (react-scripts) antes de cada arquivo de teste.
import "@testing-library/jest-dom";

beforeEach(() => {
	// jsdom não implementa fetch: cada teste começa com um mock limpo.
	global.fetch = jest.fn();
	localStorage.clear();
});
