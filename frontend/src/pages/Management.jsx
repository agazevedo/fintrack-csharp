import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";

import {
	getCategories,
	createCategory,
	updateCategory,
	deleteCategory
} from "../services/categoryService";

import {
	getBudgetItems,
	createBudgetItem,
	updateBudgetItem,
	deleteBudgetItem
} from "../services/budgetItemService";

import {
	getExpenses,
	createExpense,
	updateExpense,
	deleteExpense
} from "../services/expenseService";

export default function Management() {
	const [tab, setTab] = useState("category");

	const [categories, setCategories] = useState([]);
	const [items, setItems] = useState([]);
	const [expenses, setExpenses] = useState([]);

	const [form, setForm] = useState({});
	const [editingId, setEditingId] = useState(null);

	useEffect(() => {
		loadAll();
	}, []);

	async function loadAll() {
		setCategories(await getCategories());
		setItems(await getBudgetItems());
		setExpenses(await getExpenses());
	}

	function reset() {
		setForm({});
		setEditingId(null);
	}

	// CATEGORY
	async function handleSaveCategory() {
		try {
			const data = {
				name: form.name,
				type: form.type
			};

			if (editingId && tab === "category") {
				await updateCategory(editingId, data);
			} else {
				await createCategory(data);
			}

			reset();
			loadAll();
		} catch (err) {
			console.error(err);
			alert(err.message || "Erro ao salvar categoria");
		}
	}

	async function handleDeleteCategory(id) {
		await deleteCategory(id);
		loadAll();
	}

	// ITEM
	async function handleSaveItem() {
		const data = {
			description: form.description,
			unitValue: Number(form.unit_value),
			quantity: Number(form.quantity),
			categoryId: Number(form.categoryId)
		};

		if (editingId && tab === "item") {
			await updateBudgetItem(editingId, data);
		} else {
			await createBudgetItem(data);
		}

		reset();
		loadAll();
	}

	async function handleDeleteItem(id) {
		await deleteBudgetItem(id);
		loadAll();
	}

	// EXPENSE
	async function handleSaveExpense() {
		const data = {
			budgetItemId: Number(form.budget_item),
			unitValue: Number(form.unit_value),
			quantity: Number(form.quantity),
			date: form.date
		};

		if (editingId && tab === "expense") {
			await updateExpense(editingId, data);
		} else {
			await createExpense(data);
		}

		reset();
		loadAll();
	}

	async function handleDeleteExpense(id) {
		await deleteExpense(id);
		loadAll();
	}

	return (
		<div>
			<Navbar />

			<h1>Gerenciamento</h1>

			<div className="manager-box">
				{/* TABS */}
				<div className="form-actions">
					<button onClick={() => setTab("category")}>Categoria</button>
					<button onClick={() => setTab("item")}>Item</button>
					<button onClick={() => setTab("expense")}>Despesa</button>
				</div>

				{/* CATEGORY */}
				{tab === "category" && (
					<div className="form-section">
						<h3>Categoria</h3>

						<input
							placeholder="Nome"
							value={form.name || ""}
							onChange={e => setForm({ ...form, name: e.target.value })}
						/>

						<select
							value={form.type || ""}
							onChange={e => setForm({ ...form, type: e.target.value })}
						>
							<option value="custeio">Custeio</option>
							<option value="capital">Capital</option>
						</select>

						<button onClick={handleSaveCategory}>
							{editingId ? "Atualizar" : "Salvar"}
						</button>

						<div id="list-container">
							{categories.map(c => (
								<div key={c.id} className="list-item">
									<span>{c.name} ({c.type})</span>
									<div>
										<button onClick={() => { setForm(c); setEditingId(c.id); }}>
											Editar
										</button>
										<button onClick={() => handleDeleteCategory(c.id)}>
											Excluir
										</button>
									</div>
								</div>
							))}
						</div>
					</div>
				)}

				{/* ITEM */}
				{tab === "item" && (
					<div className="form-section">
						<h3>Item</h3>

						<input
							placeholder="Descrição"
							value={form.description || ""}
							onChange={e => setForm({ ...form, description: e.target.value })}
						/>

						<input
							type="number"
							placeholder="Valor unitário"
							value={form.unit_value || ""}
							onChange={e => setForm({ ...form, unit_value: e.target.value })}
						/>

						<input
							type="number"
							placeholder="Quantidade"
							value={form.quantity || ""}
							onChange={e => setForm({ ...form, quantity: e.target.value })}
						/>

						<select
							value={form.categoryId || ""}
							onChange={e => setForm({ ...form, categoryId: e.target.value })}
						>
							<option value="">Categoria</option>
							{categories.map(c => (
								<option key={c.id} value={c.id}>
									{c.name}
								</option>
							))}
						</select>

						<button onClick={handleSaveItem}>
							{editingId ? "Atualizar" : "Salvar"}
						</button>

						<div id="list-container">
							{items.map(i => (
								<div key={i.id} className="list-item">
									<span>{i.description}</span>
									<div>
										<button onClick={() => { setForm(i); setEditingId(i.id); }}>
											Editar
										</button>
										<button onClick={() => handleDeleteItem(i.id)}>
											Excluir
										</button>
									</div>
								</div>
							))}
						</div>
					</div>
				)}

				{/* EXPENSE */}
				{tab === "expense" && (
					<div className="form-section">
						<h3>Despesa</h3>

						<select
							value={form.budget_item || ""}
							onChange={e => setForm({ ...form, budget_item: e.target.value })}
						>
							<option value="">Item</option>
							{items.map(i => (
								<option key={i.id} value={i.id}>
									{i.description}
								</option>
							))}
						</select>

						<input
							type="number"
							placeholder="Valor"
							value={form.unit_value || ""}
							onChange={e => setForm({ ...form, unit_value: e.target.value })}
						/>

						<input
							type="number"
							placeholder="Quantidade"
							value={form.quantity || ""}
							onChange={e => setForm({ ...form, quantity: e.target.value })}
						/>

						<input
							type="date"
							value={form.date || ""}
							onChange={e => setForm({ ...form, date: e.target.value })}
						/>

						<button onClick={handleSaveExpense}>
							{editingId ? "Atualizar" : "Salvar"}
						</button>

						<div id="list-container">
							{expenses.map(e => (
								<div key={e.id} className="list-item">
									<span>{e.budgetItem?.description} ({e.budgetItem?.category?.name}) - R$ {e.total}</span>
									<div>
										<button onClick={() => { setForm(e); setEditingId(e.id); }}>
											Editar
										</button>
										<button onClick={() => handleDeleteExpense(e.id)}>
											Excluir
										</button>
									</div>
								</div>
							))}
						</div>
					</div>
				)}
			</div>
		</div>
	);
}
