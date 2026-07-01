import { useState } from "react";

export default function Modal({isOpen, onClose, title, summary, expenses, items}) {
	const [tab, setTab] = useState("summary");

	if (!isOpen) return null;

	console.log("expenses", expenses);
	console.log("items", items);
	console.log("tab", tab);

	return (
		<div className="modal">
			<div className="modal-content">

				<div className="modal-header">
					<h2>{title}</h2>
					<button onClick={onClose}>✖</button>
				</div>

				<div className="modal-tabs">
					<button className="tab-btn" onClick={() => setTab("summary")}>Resumo</button>
					<button className="tab-btn" onClick={() => setTab("expenses")}>Despesas</button>
					<button className="tab-btn" onClick={() => setTab("items")}>Itens</button>
				</div>

				<div className="modal-body">
					{tab === "summary" && (
						<div>
							<p>Total gasto: <strong>R$ {(summary.total || 0).toFixed(2)}</strong></p>
							<p>Quantidade de despesas: {summary.count || 0}</p>
						</div>
					)}

					{tab === "expenses" && (
						<div className="tab-content">
							<table>
								<thead>
									<tr>
										<th>Data</th>
										<th>Item</th>
										<th>Valor</th>
									</tr>
								</thead>

								<tbody>
									{expenses.map(e => (
										<tr key={e.id}>
											<td>{e.date}</td>
											<td>{e.budgetItem?.description}</td>
											<td>R$ {e.total}</td>
										</tr>
									))}
								</tbody>
							</table>
						</div>
					)}

					{tab === "items" && (
						<div className="tab-content">
							<table>
								<thead>
									<tr>
										<th>Item</th>
										<th>Total gasto</th>
									</tr>
								</thead>

								<tbody>
									{items.map(i => (
										<tr key={i.description}>
											<td>{i.description}</td>
											<td>
												R$ {i.total.toFixed(2)}
											</td>
										</tr>
									))}
								</tbody>
							</table>
						</div>
					)}
				</div>
			</div>
		</div>
	);
}
