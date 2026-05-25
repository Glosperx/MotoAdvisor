import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Navbar = () => {
  const { isAuthenticated, isAdmin, user, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <nav className="navbar">
      <Link to="/" className="navbar-brand">
        Moto<span>Advisor</span>
      </Link>

      <div className="navbar-links">
        <Link to="/">Catalog</Link>
        {isAuthenticated && <Link to="/search">Cauta</Link>}
        {isAuthenticated && <Link to="/profile">My Favorites</Link>}
        {isAdmin && <Link to="/admin" className="nav-admin">Admin</Link>}

        <div className="nav-divider" />

        {isAuthenticated ? (
          <>
            <span className="nav-username">{user?.userName}</span>
            <button onClick={() => { logout(); navigate('/'); }} className="btn-nav-logout">
              Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register" className="btn-nav-register">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
