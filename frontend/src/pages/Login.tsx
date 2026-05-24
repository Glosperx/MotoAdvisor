import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../contexts/AuthContext';
import type { AuthResponse } from '../types';

const Login = () => {
  const { login } = useAuth();
  const navigate  = useNavigate();
  const location  = useLocation();
  const from = (location.state as any)?.from?.pathname ?? '/';

  const [form, setForm]       = useState({ email: '', password: '' });
  const [error, setError]     = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => { document.title = 'MotoAdvisor - Login'; }, []);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await api.post<AuthResponse>('/api/auth/login', form);
      login(res.data);
      navigate(from, { replace: true });
    } catch {
      setError('Invalid email or password.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-logo">Moto<span>Advisor</span></div>
        <h1 className="auth-title">Welcome back</h1>
        <p className="auth-subtitle">Sign in to your account to continue</p>

        {error && <div className="auth-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="field">
            <label className="field-label">Email address</label>
            <input className="field-input" type="email" placeholder="you@example.com" required
              value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
          </div>
          <div className="field">
            <label className="field-label">Password</label>
            <input className="field-input" type="password" placeholder="••••••••" required
              value={form.password} onChange={e => setForm(f => ({ ...f, password: e.target.value }))} />
          </div>
          <button type="submit" className="btn-auth" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>

        <p className="auth-footer">
          No account? <Link to="/register">Create one</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;
