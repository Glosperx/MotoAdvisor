import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';
import type { Brand, Category, MotorcycleSummary } from '../types';

const licenseBadge = (lc?: string) => {
  if (!lc) return null;
  const cls = lc === 'A1' ? 'badge-a1' : lc === 'A2' ? 'badge-a2' : 'badge-a';
  return <span className={`badge-license ${cls}`}>{lc}</span>;
};

const Home = () => {
  const [motorcycles, setMotorcycles] = useState<MotorcycleSummary[]>([]);
  const [brands, setBrands]           = useState<Brand[]>([]);
  const [categories, setCategories]   = useState<Category[]>([]);
  const [filters, setFilters] = useState({
    brandId: '', categoryId: '', minPrice: '', maxPrice: '', licenseCategory: '',
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => { document.title = 'MotoAdvisor - Motorcycle Catalog'; }, []);

  useEffect(() => {
    Promise.all([
      api.get<Brand[]>('/api/brands'),
      api.get<Category[]>('/api/categories'),
    ]).then(([b, c]) => {
      setBrands(b.data);
      setCategories(c.data);
    });
  }, []);

  useEffect(() => {
    setLoading(true);
    const params = new URLSearchParams();
    if (filters.brandId)    params.append('brandId', filters.brandId);
    if (filters.categoryId) params.append('categoryId', filters.categoryId);
    if (filters.minPrice)   params.append('minPrice', filters.minPrice);
    if (filters.maxPrice)   params.append('maxPrice', filters.maxPrice);

    api.get<MotorcycleSummary[]>(`/api/motorcycles?${params}`)
      .then(r => {
        let data = r.data;
        if (filters.licenseCategory)
          data = data.filter(m => m.licenseCategory === filters.licenseCategory);
        setMotorcycles(data);
      })
      .finally(() => setLoading(false));
  }, [filters]);

  const set = (key: string, val: string) => setFilters(f => ({ ...f, [key]: val }));
  const clear = () => setFilters({ brandId: '', categoryId: '', minPrice: '', maxPrice: '', licenseCategory: '' });
  const hasFilters = Object.values(filters).some(Boolean);

  return (
    <>
      {/* Hero */}
      <section className="hero">
        <div className="hero-eyebrow">🏍 Motorcycle Catalog</div>
        <h1>Find Your <em>Perfect</em> Ride</h1>
        <p className="hero-sub">
          Browse 50 motorcycles across 11 brands. Filter by licence category,
          budget and style to find the bike that fits your life.
        </p>
      </section>

      {/* Catalog */}
      <div className="catalog-layout">
        {/* Sidebar */}
        <aside className="sidebar">
          <div className="filter-card">
            <p className="filter-title">Filters</p>

            <div className="filter-group">
              <div>
                <span className="filter-label">Brand</span>
                <div className="filter-select-wrap">
                  <select className="filter-select" value={filters.brandId} onChange={e => set('brandId', e.target.value)}>
                    <option value="">All brands</option>
                    {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
                  </select>
                </div>
              </div>

              <div>
                <span className="filter-label">Category</span>
                <div className="filter-select-wrap">
                  <select className="filter-select" value={filters.categoryId} onChange={e => set('categoryId', e.target.value)}>
                    <option value="">All categories</option>
                    {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
              </div>

              <div>
                <span className="filter-label">Licence Category</span>
                <div className="filter-select-wrap">
                  <select className="filter-select" value={filters.licenseCategory} onChange={e => set('licenseCategory', e.target.value)}>
                    <option value="">Any licence</option>
                    <option value="A1">A1 — up to 11 kW</option>
                    <option value="A2">A2 — up to 35 kW</option>
                    <option value="A">A — Unlimited</option>
                  </select>
                </div>
              </div>

              <div>
                <span className="filter-label">Min Price (€)</span>
                <input className="filter-input" type="number" min="0" placeholder="0"
                  value={filters.minPrice} onChange={e => set('minPrice', e.target.value)} />
              </div>

              <div>
                <span className="filter-label">Max Price (€)</span>
                <input className="filter-input" type="number" min="0" placeholder="Any"
                  value={filters.maxPrice} onChange={e => set('maxPrice', e.target.value)} />
              </div>
            </div>

            {hasFilters && (
              <button className="btn-clear" onClick={clear}>✕ Clear filters</button>
            )}
          </div>
        </aside>

        {/* Grid */}
        <section className="catalog-main">
          <div className="catalog-header">
            <h2>Motorcycles</h2>
            {!loading && <span className="catalog-count">{motorcycles.length} results</span>}
          </div>

          {loading ? (
            <p className="loading">Loading…</p>
          ) : motorcycles.length === 0 ? (
            <p className="empty">No motorcycles match your filters.</p>
          ) : (
            <div className="moto-grid">
              {motorcycles.map(m => (
                <Link to={`/motorcycles/${m.id}`} key={m.id} className="moto-card">
                  <div className="moto-card-img-wrap">
                    {m.mainImageUrl
                      ? <img src={m.mainImageUrl} alt={m.name} loading="lazy" />
                      : <div className="moto-card-img-placeholder">🏍️</div>}
                    <div className="card-badges">
                      {licenseBadge(m.licenseCategory)}
                      {m.isBeginnerFriendly && (
                        <span className="badge-beginner">Beginner</span>
                      )}
                    </div>
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
          )}
        </section>
      </div>
    </>
  );
};

export default Home;
