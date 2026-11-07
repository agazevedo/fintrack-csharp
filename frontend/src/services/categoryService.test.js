import { createCategory, deleteCategory, getCategories, updateCategory } from "./categoryService";
import { describeCrudService } from "../testUtils/crudServiceSuite";

describeCrudService({
	name: "categoryService",
	endpoint: "/categories",
	service: {
		getAll: getCategories,
		create: createCategory,
		update: updateCategory,
		remove: deleteCategory
	},
	payload: { name: "Mercado", type: "expense" },
	messages: {
		getAll: "Erro ao buscar categorias",
		update: "Erro ao atualizar categoria",
		delete: "Erro ao deletar categoria"
	}
});
