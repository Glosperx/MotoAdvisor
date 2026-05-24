import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../contexts/AuthContext';
import Gallery from '../components/Gallery';
import type { Favorite, MotorcycleDetail as MotorcycleDetailType, Review } from '../types';

const starsFill = (rating: number) => {
  const full = Math.round(rating);
  return '★'.repeat(full) + '☆'.repeat(5 - full);
};

const licenseBadgeClass = (lc?: string) =>
  lc === 'A1' ? 'badge-detail-a1' : lc === 'A2' ? 'badge-detail-a2' : 'badge-detail-a';

const MotorcycleDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [moto, setMoto]           = useState<MotorcycleDetailType | null>(null);
  const [reviews, setReviews]     = useState<Review[]>([]);
  const [isFav, setIsFav]         = useState(false);
  const [draft, setDraft]         = useState({ rating: 5, content: '' });
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading]     = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const [mRes, rRes] = await Promise.all([
          api.get<MotorcycleDetailType>(`/api/motorcycles/${id}`),
          api.get<Review[]>(`/api/motorcycles/${id}/reviews`),
        ]);
        setMoto(mRes.data);
        setReviews(rRes.data);
        if (isAuthenticated) {
          const favRes = await api.get<Favorite[]>('/api/favorites');
          setIsFav(favRes.data.some(f => f.motorcycleId === Number(id)));
        }
      } catch {
        navigate('/');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [id, isAuthenticated, navigate]);

  const toggleFav = async () => {
    if (!isAuthenticated) { navigate('/login'); return; }
    if (isFav) await api.delete(`/api/favorites/${id}`);
    else       await api.post(`/api/favorites/${id}`);
    setIsFav(f => !f);
  };

  const submitReview = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await api.post(`/api/motorcycles/${id}/reviews`, draft);
      const res = await api.get<Review[]>(`/api/motorcycles/${id}/reviews`);
      setReviews(res.data);
      setDraft({ rating: 5, content: '' });
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <p className="loading">Loading…</p>;
  if (!moto)   return <p className="empty">Motorcycle not found.</p>;

  document.title = `MotoAdvisor - ${moto.name}`;

  const specs: [string, string | number | undefined, boolean?][] = [
    ['Year',    moto.year],
    ['Engine',  moto.engine],
    ['Power',   moto.power],
    ['Horsepower', moto.horsepower ? `${moto.horsepower} hp` : undefined, true],
  ];

  return (
    <div className="detail-page">
      {/* Breadcrumb */}
      <div className="detail-breadcrumb">
        <Link to="/">Catalog</Link>
        <span>›</span>
        <span>{moto.brand.name}</span>
        <span>›</span>
        <span>{moto.name}</span>
      </div>

      {/* Main grid */}
      <div className="detail-grid">
        {/* Gallery */}
        <Gallery
          images={moto.imageUrls.filter(u => !u.includes('placehold.co'))}
          alt={moto.name}
        />

        {/* Info panel */}
        <div>
          <div className="detail-brand-tag">{moto.brand.name}</div>

          <div className="detail-header">
            <h1 className="detail-name">{moto.name}</h1>
            <button className={`btn-fav ${isFav ? 'active' : ''}`} onClick={toggleFav}
              title={isFav ? 'Remove from favorites' : 'Save to favorites'}>
              {isFav ? '❤️' : '🤍'}
            </button>
          </div>

          {/* Badges */}
          <div className="detail-badges">
            {moto.licenseCategory && (
              <span className={`badge-detail ${licenseBadgeClass(moto.licenseCategory)}`}>
                Licence {moto.licenseCategory}
              </span>
            )}
            {moto.isBeginnerFriendly && (
              <span className="badge-detail badge-detail-beg">Beginner Friendly</span>
            )}
            <span style={{ fontSize: '.78rem', color: 'var(--muted)', padding: '.3rem 0' }}>
              {moto.category.name} · {moto.year}
            </span>
          </div>

          {/* Price */}
          <div className="detail-price">
            <span className="detail-price-currency">€</span>
            {moto.price.toLocaleString()}
          </div>

          {/* Rating */}
          {moto.averageRating != null && (
            <div className="detail-rating">
              <span className="stars">{starsFill(moto.averageRating)}</span>
              <span className="rating-val">{moto.averageRating.toFixed(1)}</span>
              <span className="rating-count">({moto.reviewCount} {moto.reviewCount === 1 ? 'review' : 'reviews'})</span>
            </div>
          )}

          {/* Spec grid */}
          <div className="spec-grid">
            {specs.filter(([, v]) => v !== undefined && v !== 0).map(([label, value, accent]) => (
              <div key={label} className={`spec-item ${accent ? 'accent' : ''}`}>
                <div className="spec-label">{label}</div>
                <div className="spec-value">{String(value)}</div>
              </div>
            ))}
          </div>

          {/* Description */}
          {moto.description && (
            <p className="detail-description">{moto.description}</p>
          )}
        </div>
      </div>

      {/* Reviews */}
      <div className="reviews-section">
        <h2 className="reviews-title">Reviews ({reviews.length})</h2>

        {isAuthenticated && (
          <form className="review-form" onSubmit={submitReview}>
            <p className="review-form-title">Leave a review</p>
            <div className="review-rating-row">
              <span className="review-rating-label">Rating</span>
              <select className="review-rating-select"
                value={draft.rating}
                onChange={e => setDraft(d => ({ ...d, rating: Number(e.target.value) }))}>
                {[5, 4, 3, 2, 1].map(n => (
                  <option key={n} value={n}>{'★'.repeat(n)}{'☆'.repeat(5 - n)} {n}/5</option>
                ))}
              </select>
            </div>
            <textarea className="review-textarea"
              value={draft.content}
              onChange={e => setDraft(d => ({ ...d, content: e.target.value }))}
              placeholder="Share your experience with this motorcycle…"
            />
            <button type="submit" className="btn-submit-review" disabled={submitting}>
              {submitting ? 'Submitting…' : 'Submit Review'}
            </button>
          </form>
        )}

        {reviews.length === 0
          ? <p className="empty" style={{ padding: '2rem 0' }}>No reviews yet — be the first!</p>
          : reviews.map(r => (
            <div key={r.id} className="review-card">
              <div className="review-header">
                <span className="review-author">{r.userName}</span>
                <span className="review-date">{new Date(r.createdAt).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })}</span>
              </div>
              <div className="review-stars" style={{ color: '#f59e0b' }}>
                {'★'.repeat(r.rating)}{'☆'.repeat(5 - r.rating)}
              </div>
              {r.content && <p className="review-content">{r.content}</p>}
            </div>
          ))
        }
      </div>
    </div>
  );
};

export default MotorcycleDetail;
