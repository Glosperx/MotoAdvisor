import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../contexts/AuthContext';
import type { Favorite } from '../types';

const Profile = () => {
  const { user } = useAuth();
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [loading, setLoading]     = useState(true);

  useEffect(() => { document.title = 'MotoAdvisor - My Profile'; }, []);

  useEffect(() => {
    api.get<Favorite[]>('/api/favorites')
      .then(r => setFavorites(r.data))
      .finally(() => setLoading(false));
  }, []);

  const remove = async (motorcycleId: number) => {
    await api.delete(`/api/favorites/${motorcycleId}`);
    setFavorites(fs => fs.filter(f => f.motorcycleId !== motorcycleId));
  };

  const initial = user?.userName?.[0]?.toUpperCase() ?? '?';

  return (
    <div className="profile-page">
      {/* Profile hero */}
      <div className="profile-hero">
        <div className="profile-avatar">{initial}</div>
        <div className="profile-info">
          <h2>{user?.userName}</h2>
          <p>{user?.email}</p>
          {user?.roles?.includes('Admin') && (
            <span className="badge-admin">Admin</span>
          )}
        </div>
      </div>

      {/* Favorites */}
      <p className="profile-section-title">
        Saved Motorcycles
        <span style={{ fontWeight: 400, color: 'var(--muted)', fontSize: '.875rem', marginLeft: '.5rem' }}>
          ({favorites.length})
        </span>
      </p>

      {loading ? (
        <p className="loading">Loading…</p>
      ) : favorites.length === 0 ? (
        <p className="empty">
          No favorites yet.{' '}
          <Link to="/" style={{ color: 'var(--orange)', fontWeight: 600 }}>Browse the catalog</Link>
        </p>
      ) : (
        <div className="grid">
          {favorites.map(f => (
            <div key={f.motorcycleId} className="fav-card">
              <div className="fav-img-wrap">
                {f.mainImageUrl
                  ? <img src={f.mainImageUrl} alt={f.name} />
                  : <div className="fav-img-placeholder">🏍️</div>}
              </div>
              <div className="fav-body">
                <div className="fav-name">{f.name}</div>
                <div className="fav-sub">{f.brandName} · {f.year}</div>
                <div className="fav-price">€{f.price.toLocaleString()}</div>
                <div className="fav-actions">
                  <Link to={`/motorcycles/${f.motorcycleId}`} className="btn-view">View</Link>
                  <button className="btn-remove" onClick={() => remove(f.motorcycleId)}>Remove</button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Profile;
