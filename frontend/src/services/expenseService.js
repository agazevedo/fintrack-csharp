import { fetchWithAuth } from "../api/api";

export async function getExpenses() {
	const res = await fetchWithAuth("/expenses");

	if (!res.ok) {
		throw new Error("Erro ao buscar despesas");
	}

	return await res.json();
}

export async function createExpense(data) {
	const res = await fetchWithAuth("/expenses", {
		method: "POST",
		body: JSON.stringify(data)
	});

	if (!res.ok) {
		const text = await res.text();
		throw new Error(text || "Erro ao criar despesa");
	}

	return await res.json();
}

export async function updateExpense(id, data) {
	const res = await fetchWithAuth(`/expenses/${id}`, {
		method: "PUT",
		body: JSON.stringify(data)
	});

	if (!res.ok) {
		const text = await res.text();
		throw new Error(text || "Erro ao atualizar despesa");
	}

	return await res.json();
}

export async function deleteExpense(id) {
	const res = await fetchWithAuth(`/expenses/${id}`, {
		method: "DELETE"
	});

	if (!res.ok) {
		throw new Error("Erro ao deletar despesa");
	}

	return true;
}
