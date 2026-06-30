import { getToken } from "../services/auth";

export const API_URL = "http://localhost:5018/api";

export function getHeaders() {
	const headers = {
		"Content-Type": "application/json"
	};

	const token = getToken();

	if (token) {
		headers.Authorization = `Bearer ${token}`;
	}

	return headers;
}

export async function fetchWithAuth(endpoint, options = {}) {
	const response = await fetch(`${API_URL}${endpoint}`, {
		...options,
		headers: {
			...getHeaders(),
			...(options.headers || {})
		}
	});

	// só força logout se realmente for auth endpoint ou refresh falhou
	if (response.status === 401 && endpoint !== "/auth/login") {
		localStorage.removeItem("token");
		window.location.href = "/login";
	}

	return response;
}
