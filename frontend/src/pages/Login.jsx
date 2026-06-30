import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { login } from "../services/auth";

export default function Login() {
	const navigate = useNavigate();

	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");

	const handleSubmit = async (e) => {
		e.preventDefault();

		const success = await login(username, password);

		if (success) {
			navigate("/");
		} else {
			alert("Usuário ou senha inválidos.");
		}
	};

	return (
		<main>
			<form id="login-form" onSubmit={handleSubmit}>
				<h2>FinTrack Login</h2>

				<input
					type="text"
					placeholder="Usuário"
					value={username}
					onChange={(e) => setUsername(e.target.value)}
				/>

				<input
					type="password"
					placeholder="Senha"
					value={password}
					onChange={(e) => setPassword(e.target.value)}
				/>

				<button type="submit">
					Entrar
				</button>
			</form>
		</main>
	);
}
