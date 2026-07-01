import { Link, useNavigate } from "react-router-dom";
import { logout } from "../services/auth";

export default function Navbar() {
	const navigate = useNavigate();

	function handleLogout() {
		logout();
		navigate("/login");
	}

	return (
		<header className="navbar">
			<div className="nav-left">
				<Link to="/">FinTrack</Link>
			</div>

			<ul className="nav-right">
				<li><Link to="/management">Gerenciamento</Link></li>
				<li><Link to="/">Dashboard</Link></li>
				<li><Link to="#" onClick={handleLogout}>Logout</Link></li>
			</ul>
		</header>
	);
}
