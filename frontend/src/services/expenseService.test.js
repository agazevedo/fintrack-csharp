import { createExpense, deleteExpense, getExpenses, updateExpense } from "./expenseService";
import { describeCrudService } from "../testUtils/crudServiceSuite";

describeCrudService({
	name: "expenseService",
	endpoint: "/expenses",
	service: {
		getAll: getExpenses,
		create: createExpense,
		update: updateExpense,
		remove: deleteExpense
	},
	payload: { budgetItemId: 1, unitValue: 80, quantity: 2, date: "2026-11-01" },
	messages: {
		getAll: "Erro ao buscar despesas",
		update: "Erro ao atualizar despesa",
		delete: "Erro ao deletar despesa"
	}
});
