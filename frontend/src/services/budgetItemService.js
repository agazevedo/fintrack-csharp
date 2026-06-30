import { fetchWithAuth } from "../api/api";

export async function getBudgetItems() {
	const res = await fetchWithAuth("/budget-items");

	if (!res.ok) {
		throw new Error("Erro ao buscar itens");
	}

	return await res.json();
}

export async function createBudgetItem(data) {
	const res = await fetchWithAuth("/budget-items", {
		method: "POST",
		body: JSON.stringify(data)
	});

	if (!res.ok) {
		const text = await res.text();
		throw new Error(text || "Erro ao criar item");
	}

	return await res.json();
}

export async function updateBudgetItem(id, data) {
	const res = await fetchWithAuth(`/budget-items/${id}`, {
		method: "PUT",
		body: JSON.stringify(data)
	});

	if (!res.ok) {
		const text = await res.text();
		throw new Error(text || "Erro ao atualizar item");
	}

	return await res.json();
}

export async function deleteBudgetItem(id) {
	const res = await fetchWithAuth(`/budget-items/${id}`, {
		method: "DELETE"
	});

	if (!res.ok) {
		throw new Error("Erro ao deletar item");
	}

	return true;
}
