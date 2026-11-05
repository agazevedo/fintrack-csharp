import { createBudgetItem, deleteBudgetItem, getBudgetItems, updateBudgetItem } from "./budgetItemService";
import { describeCrudService } from "../testUtils/crudServiceSuite";

describeCrudService({
	name: "budgetItemService",
	endpoint: "/budget-items",
	service: {
		getAll: getBudgetItems,
		create: createBudgetItem,
		update: updateBudgetItem,
		remove: deleteBudgetItem
	},
	payload: { categoryId: 1, description: "Aluguel", unitValue: 1500, quantity: 1 },
	messages: {
		getAll: "Erro ao buscar itens",
		update: "Erro ao atualizar item",
		delete: "Erro ao deletar item"
	}
});
