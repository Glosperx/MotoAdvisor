import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../contexts/AuthContext';
import type { AuthResponse } from '../types';

const Register = () => {
  const { login } = useAuth();
  const navigate  = useNavigate();

  const [form, setForm]       = useState({ userName: '', email: '', password: '' });
  const [errors, setErrors]   = useState<string[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { document.title = 'MotoAdvisor - Register'; }, []);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setErrors([]);
    setLoading(true);
    try {
      const res = await api.post<AuthResponse>('/api/auth/register', form);
      login(res.data);
      navigate('/');
    } catch (err: any) {
      const errs = err.response?.data?.errors;
      setErrors(Array.isArray(errs) ? errs : ['Registration failed. Please try again.']);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-logo">Moto<span>Advisor</span></div>
        <h1 className="auth-title">Create an account</h1>
        <p className="auth-subtitle">Join thousands of motorcycle enthusiasts</p>

        {errors.map((e, i) => <div key={i} className="auth-error">{e}</div>)}

        <form onSubmit={handleSubmit}>
          <div className="field">
            <label className="field-label">Username</label>
            <input className="field-input" placeholder="yourname" required
              value={form.userName} onChange={e => setForm(f => ({ ...f, userName: e.target.value }))} />
          </div>
          <div className="field">
            <label className="field-label">Email address</label>
            <input className="field-input" type="email" placeholder="you@example.com" required
              value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
          </div>
          <div className="field">
            <label className="field-label">Password</label>
            <input className="field-input" type="password" placeholder="Min 6 characters" required minLength={6}
              value={form.password} onChange={e => setForm(f => ({ ...f, password: e.target.value }))} />
          </div>
          <button type="submit" className="btn-auth" disabled={loading}>
            {loading ? 'Creating account…' : 'Create account'}
          </button>
        </form>

        <p className="auth-footer">
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </div>
    </div>
  );
};

export default Register;
