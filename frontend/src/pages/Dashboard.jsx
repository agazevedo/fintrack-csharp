import { useEffect, useRef, useState } from "react";
import { getBudgetItems } from "../services/budgetItemService";
import { getExpenses } from "../services/expenseService";
import Chart from "chart.js/auto";
import Navbar from "../components/Navbar";
import Modal from "../components/Modal";

export default function Dashboard() {
	const [expenses, setExpenses] = useState([]);
	const [modalOpen, setModalOpen] = useState(false);
	const [selectedCategory, setSelectedCategory] = useState("");
	const [modalData, setModalData] = useState({
		summary: {},
		expenses: [],
		items: []
	});

	const financeRef = useRef(null);
	const categoryRef = useRef(null);

	const financeChartRef = useRef(null);
	const categoryChartRef = useRef(null);

	const [summary, setSummary] = useState({
		totalPlanned: 0,
		totalSpent: 0,
		totalRemaining: 0
	});

	useEffect(() => {
		async function loadDashboard() {
			try {
				const items = await getBudgetItems();
				const expenseList = await getExpenses();

				console.log(expenseList);
				setExpenses(expenseList);

				let totalPlanned = 0;
				let totalSpent = 0;

				const categoryMap = {};

				items.forEach(item => {
					totalPlanned += Number(item.budgetTotal ?? 0);
				});

				expenseList.forEach(exp => {
					totalSpent += Number(exp.total ?? 0);

					const cat = exp.budgetItem?.category?.name;
					if (!categoryMap[cat]) categoryMap[cat] = 0;
					categoryMap[cat] += Number(exp.total);
				});

				setSummary({
					totalPlanned,
					totalSpent,
					totalRemaining: totalPlanned - totalSpent
				});

				renderFinanceChart(totalSpent, totalPlanned - totalSpent);
				renderCategoryChart(categoryMap, expenseList);

			} catch (err) {
				console.error(err);
			}
		}

		loadDashboard();

		return () => {
			if (financeChartRef.current) financeChartRef.current.destroy();
			if (categoryChartRef.current) categoryChartRef.current.destroy();
		};
	}, []);

	function renderFinanceChart(spent, remaining) {
		if (!financeRef.current) return;

		if (financeChartRef.current) {
			financeChartRef.current.destroy();
		}

		const ctx = financeRef.current.getContext("2d");

		financeChartRef.current = new Chart(ctx, {
			type: "doughnut",
			data: {
				labels: ["Gasto", "Restante"],
				datasets: [
					{
						data: [spent, remaining],
						backgroundColor: ["#e74c3c", "#2ecc71"]
					}
				]
			},
			options: {
				responsive: true,
				plugins: {
					legend: {
						position: "bottom"
					}
				}
			}
		});
	}

	function renderCategoryChart(map, expenseList) {
		if (!categoryRef.current) return;

		if (categoryChartRef.current) {
			categoryChartRef.current.destroy();
		}

		const labels = Object.keys(map);
		const values = Object.values(map);

		const ctx = categoryRef.current.getContext("2d");

		categoryChartRef.current = new Chart(ctx, {
			type: "pie",
			data: {
				labels,
				datasets: [
					{
						data: values,
						backgroundColor: labels.map(
							(_, i) => `hsl(${(i * 360) / labels.length}, 70%, 60%)`
						)
					}
				]
			},
			options: {
				responsive: true,
				onClick: (evt, elements) => {
					if (!elements.length) return;

					const index = elements[0].index;

					openCategoryModal(labels[index], expenseList);
				}
			}
		});
	}

	function openCategoryModal(categoryName, expenseList) {
		const filtered = expenseList.filter(
			e => e.budgetItem?.category?.name === categoryName
		);

		let total = 0;

		filtered.forEach(e => {
			total += Number(e.total);
		});

		const itemMap = {};

		filtered.forEach(e => {
			const description = e.budgetItem?.description;

			if (!itemMap[description]) itemMap[description] = 0;

			itemMap[description] += Number(e.total);
		});

		const items = Object.entries(itemMap).map(([description, total]) => ({
			description,
			total
		}));

		setSelectedCategory(categoryName);

		setModalData({
			summary: {
				total,
				count: filtered.length
			},
			expenses: filtered,
			items
		});

		setModalOpen(true);
	}

	return (
		<div>
			<Navbar />

			<div className="manager-box">
				<h1>Dashboard</h1>

				{/* SUMMARY CARDS */}
				<div className="charts-container">
					<div className="card">
						<h3>Previsto</h3>
						<strong>R$ {summary.totalPlanned.toFixed(2)}</strong>
					</div>

					<div className="card">
						<h3>Gasto</h3>
						<strong>R$ {summary.totalSpent.toFixed(2)}</strong>
					</div>

					<div className="card">
						<h3>Restante</h3>
						<strong>R$ {summary.totalRemaining.toFixed(2)}</strong>
					</div>
				</div>

				{/* CHARTS */}
				<div className="charts-container">
					<div className="card" style={{ flex: 1 }}>
						<h3>Gastos por categoria</h3>
						<canvas ref={categoryRef}></canvas>
					</div>

					<div className="card" style={{ flex: 1 }}>
						<h3>Resumo financeiro</h3>
						<canvas ref={financeRef}></canvas>
					</div>
				</div>
			</div>

			<Modal
				isOpen={modalOpen}
				onClose={() => setModalOpen(false)}
				title={selectedCategory}
				summary={modalData.summary}
				expenses={modalData.expenses}
				items={modalData.items}
			/>
		</div>
	);
}
