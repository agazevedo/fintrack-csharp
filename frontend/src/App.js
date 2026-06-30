import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import "./styles/style.css";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Management from "./pages/Management";
import ProtectedRoute from "./components/ProtectedRoute";

export default function App() {
	return (
		<BrowserRouter>
			<Routes>
				<Route path="/login" element={<Login />} />

				<Route
					path="/"
					element={
						<ProtectedRoute>
							<Dashboard />
						</ProtectedRoute>
					}
				/>

				<Route
					path="/management"
					element={
						<ProtectedRoute>
							<Management />
						</ProtectedRoute>
					}
				/>

				<Route path="*" element={<Navigate to="/" />} />
			</Routes>
		</BrowserRouter>
	);
}
