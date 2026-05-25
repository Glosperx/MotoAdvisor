import { useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';
import type { RecommendationResult } from '../types';

const Search = () => {
  const [query, setQuery] = useState('');
  const [result, setResult] = useState<RecommendationResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;

    setLoading(true);
    setError('');
    setResult(null);

    try {
      const response = await api.post<RecommendationResult>('/api/recommend', { query });
      setResult(response.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'A aparut o eroare. Incearca din nou.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="search-page">
      <section className="search-hero">
        <h1>Gaseste motocicleta perfecta</h1>
        <p>Descrie ce cauti si AI-ul nostru iti va recomanda cele mai potrivite optiuni.</p>
      </section>

      <form onSubmit={handleSubmit} className="search-form">
        <textarea
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Descrie motocicleta ideala pentru tine... (ex: 'Caut o motocicleta pentru oras, usor de manevrat, potrivita pentru incepatori, buget sub 8000 euro')"
          rows={4}
          disabled={loading}
        />
        <button type="submit" className="btn-search" disabled={loading || !query.trim()}>
          {loading ? 'Se cauta...' : 'Cauta'}
        </button>
      </form>

      {error && <div className="search-error">{error}</div>}

      {result && (
        <div className="search-results">
          <div className="ai-response">
            <h2>Recomandarea noastra</h2>
            <p>{result.aiResponse}</p>
          </div>

          <h2>Motociclete recomandate</h2>
          <div className="moto-grid">
            {result.motorcycles.map((m) => (
              <Link to={`/motorcycles/${m.id}`} key={m.id} className="moto-card">
                <div className="moto-card-img-wrap">
                  {m.mainImageUrl ? (
                    <img src={m.mainImageUrl} alt={m.name} loading="lazy" />
                  ) : (
                    <div className="moto-card-img-placeholder">🏍️</div>
                  )}
                </div>
                <div className="moto-card-body">
                  <div className="moto-card-brand">{m.brandName}</div>
                  <div className="moto-card-name">{m.name}</div>
                  <div className="moto-card-meta">
                    <span>{m.categoryName}</span>
                    <span className="moto-card-meta-dot" />
                    <span>{m.year}</span>
                  </div>
                  <div className="moto-card-footer">
                    <div className="moto-card-price">
                      <span>€</span>{m.price.toLocaleString()}
                    </div>
                    {m.horsepower > 0 && (
                      <div className="moto-card-hp">{m.horsepower} hp</div>
                    )}
                  </div>
                </div>
              </Link>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default Search;
