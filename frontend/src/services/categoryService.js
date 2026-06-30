import { fetchWithAuth } from "../api/api";

export async function getCategories() {
	const res = await fetchWithAuth("/categories");

	if (!res.ok) {
		throw new Error("Erro ao buscar categorias");
	}

	return await res.json();
}

export async function createCategory(data) {
	const res = await fetchWithAuth("/categories", {
		method: "POST",
		body: JSON.stringify(data)
	});

	if (!res.ok) {
		const text = await res.text();
		throw new Error(text || "Erro ao criar categoria");
	}

	return await res.json();
}

export async function updateCategory(id, data) {
	const res = await fetchWithAuth(`/categories/${id}`, {
		method: "PUT",
		body: JSON.stringify(data)
	});

	if (!res.ok) {
		const text = await res.text();
		throw new Error(text || "Erro ao atualizar categoria");
	}

	return await res.json();
}

export async function deleteCategory(id) {
	const res = await fetchWithAuth(`/categories/${id}`, {
		method: "DELETE"
	});

	if (!res.ok) {
		throw new Error("Erro ao deletar categoria");
	}

	return true;
}
