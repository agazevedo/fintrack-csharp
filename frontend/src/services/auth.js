import { API_URL } from "../api/api";

export async function login(username, password) {
	try {
		const response = await fetch(`${API_URL}/auth/login`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify({
				username,
				password
			})
		});

		if (!response.ok) {
			return false;
		}

		const data = await response.json();

		localStorage.setItem("token", data.token);

		return true;
	}
	catch (error) {
		console.error(error);
		return false;
	}
}

export function logout() {
	localStorage.removeItem("token");
}

export function isAuthenticated() {
	return localStorage.getItem("token") !== null;
}

export function getToken() {
	return localStorage.getItem("token");
}
