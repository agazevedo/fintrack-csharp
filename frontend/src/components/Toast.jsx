import { useEffect } from "react";

export default function Toast({ show, message, error, onClose }) {
	useEffect(() => {
		if (!show) return;

		const timer = setTimeout(onClose, 3000);

		return () => clearTimeout(timer);
	}, [show, onClose]);

	if (!show) return null;

	return (
		<div className={`toast ${error ? "error" : "show"}`}>
			{message}
		</div>
	);
}
