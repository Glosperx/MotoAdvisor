import { useEffect, useState } from 'react';
import api from '../api/axios';
import type { Brand, Category, MotorcycleSummary } from '../types';

type Tab = 'motorcycles' | 'brands' | 'categories';

const emptyMoto = { name: '', year: new Date().getFullYear(), price: 0, engine: '', power: '', description: '', brandId: 0, categoryId: 0 };
const emptyBrand = { name: '', country: '', logoUrl: '' };
const emptyCategory = { name: '', description: '' };

const AdminDashboard = () => {
  const [tab, setTab] = useState<Tab>('motorcycles');

  useEffect(() => { document.title = 'MotoAdvisor - Admin'; }, []);

  const [motorcycles, setMotorcycles] = useState<MotorcycleSummary[]>([]);
  const [brands, setBrands]           = useState<Brand[]>([]);
  const [categories, setCategories]   = useState<Category[]>([]);

  const [motoForm, setMotoForm]         = useState({ ...emptyMoto });
  const [brandForm, setBrandForm]       = useState({ ...emptyBrand });
  const [categoryForm, setCategoryForm] = useState({ ...emptyCategory });
  const [editingId, setEditingId]       = useState<number | null>(null);

  const reload = async () => {
    const [m, b, c] = await Promise.all([
      api.get<MotorcycleSummary[]>('/api/motorcycles'),
      api.get<Brand[]>('/api/brands'),
      api.get<Category[]>('/api/categories'),
    ]);
    setMotorcycles(m.data);
    setBrands(b.data);
    setCategories(c.data);
  };

  useEffect(() => { reload(); }, []);

  const cancelEdit = () => {
    setEditingId(null);
    setMotoForm({ ...emptyMoto });
    setBrandForm({ ...emptyBrand });
    setCategoryForm({ ...emptyCategory });
  };

  /* ── Motorcycles ── */
  const saveMoto = async () => {
    if (editingId) await api.put(`/api/motorcycles/${editingId}`, motoForm);
    else           await api.post('/api/motorcycles', motoForm);
    await reload();
    cancelEdit();
  };
  const deleteMoto = async (id: number) => {
    if (!confirm('Delete this motorcycle?')) return;
    await api.delete(`/api/motorcycles/${id}`);
    setMotorcycles(ms => ms.filter(m => m.id !== id));
  };
  const editMoto = (m: MotorcycleSummary) => {
    setTab('motorcycles');
    setEditingId(m.id);
    setMotoForm({ name: m.name, year: m.year, price: m.price, engine: m.engine ?? '', power: m.power ?? '', description: '', brandId: m.brandId, categoryId: m.categoryId });
  };

  /* ── Brands ── */
  const saveBrand = async () => {
    if (editingId) await api.put(`/api/brands/${editingId}`, brandForm);
    else           await api.post('/api/brands', brandForm);
    const res = await api.get<Brand[]>('/api/brands');
    setBrands(res.data);
    cancelEdit();
  };
  const deleteBrand = async (id: number) => {
    if (!confirm('Delete this brand?')) return;
    await api.delete(`/api/brands/${id}`);
    setBrands(bs => bs.filter(b => b.id !== id));
  };
  const editBrand = (b: Brand) => {
    setTab('brands');
    setEditingId(b.id);
    setBrandForm({ name: b.name, country: b.country ?? '', logoUrl: b.logoUrl ?? '' });
  };

  /* ── Categories ── */
  const saveCategory = async () => {
    if (editingId) await api.put(`/api/categories/${editingId}`, categoryForm);
    else           await api.post('/api/categories', categoryForm);
    const res = await api.get<Category[]>('/api/categories');
    setCategories(res.data);
    cancelEdit();
  };
  const deleteCategory = async (id: number) => {
    if (!confirm('Delete this category?')) return;
    await api.delete(`/api/categories/${id}`);
    setCategories(cs => cs.filter(c => c.id !== id));
  };
  const editCategory = (c: Category) => {
    setTab('categories');
    setEditingId(c.id);
    setCategoryForm({ name: c.name, description: c.description ?? '' });
  };

  const tabBtn = (t: Tab, label: string) => (
    <button key={t} onClick={() => { setTab(t); cancelEdit(); }}
      style={{ padding: '.5rem 1.25rem', border: 'none', cursor: 'pointer', borderRadius: '4px 4px 0 0', fontWeight: tab === t ? 700 : 400, background: tab === t ? 'var(--primary)' : '#e0e0e0', color: tab === t ? '#fff' : 'var(--text)' }}>
      {label}
    </button>
  );

  const inputStyle = { padding: '.5rem .7rem', border: '1px solid var(--border)', borderRadius: '4px', fontSize: '.9rem', width: '100%' };

  return (
    <div className="page">
      <h2 className="page-title">Admin Dashboard</h2>

      <div style={{ display: 'flex', gap: '.25rem', marginBottom: '1.5rem', borderBottom: '2px solid var(--primary)' }}>
        {tabBtn('motorcycles', `Motorcycles (${motorcycles.length})`)}
        {tabBtn('brands', `Brands (${brands.length})`)}
        {tabBtn('categories', `Categories (${categories.length})`)}
      </div>

      {/* ─────── Motorcycles ─────── */}
      {tab === 'motorcycles' && (
        <>
          <div style={{ background: '#fff', border: '1px solid var(--border)', borderRadius: '8px', padding: '1.25rem', marginBottom: '1.5rem' }}>
            <h3 style={{ marginBottom: '1rem', fontSize: '1rem' }}>{editingId ? 'Edit' : 'Add'} Motorcycle</h3>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '.75rem' }}>
              <input placeholder="Name" value={motoForm.name} onChange={e => setMotoForm(f => ({ ...f, name: e.target.value }))} style={inputStyle} />
              <input type="number" placeholder="Year" value={motoForm.year} onChange={e => setMotoForm(f => ({ ...f, year: +e.target.value }))} style={inputStyle} />
              <input type="number" placeholder="Price (€)" value={motoForm.price} onChange={e => setMotoForm(f => ({ ...f, price: +e.target.value }))} style={inputStyle} />
              <input placeholder="Engine" value={motoForm.engine} onChange={e => setMotoForm(f => ({ ...f, engine: e.target.value }))} style={inputStyle} />
              <input placeholder="Power" value={motoForm.power} onChange={e => setMotoForm(f => ({ ...f, power: e.target.value }))} style={inputStyle} />
              <select value={motoForm.brandId} onChange={e => setMotoForm(f => ({ ...f, brandId: +e.target.value }))} style={inputStyle}>
                <option value={0}>Select Brand</option>
                {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
              <select value={motoForm.categoryId} onChange={e => setMotoForm(f => ({ ...f, categoryId: +e.target.value }))} style={inputStyle}>
                <option value={0}>Select Category</option>
                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <textarea placeholder="Description (optional)" value={motoForm.description} onChange={e => setMotoForm(f => ({ ...f, description: e.target.value }))}
              style={{ ...inputStyle, marginTop: '.75rem', minHeight: '70px', resize: 'vertical', display: 'block' }} />
            <div style={{ display: 'flex', gap: '.5rem', marginTop: '.75rem' }}>
              <button onClick={saveMoto} className="btn btn-success">{editingId ? 'Update' : 'Add'} Motorcycle</button>
              {editingId && <button onClick={cancelEdit} className="btn btn-ghost">Cancel</button>}
            </div>
          </div>

          <table className="table">
            <thead><tr>
              <th>Name</th><th>Brand</th><th>Category</th><th>Year</th><th style={{ textAlign: 'right' }}>Price</th><th></th>
            </tr></thead>
            <tbody>
              {motorcycles.map(m => (
                <tr key={m.id}>
                  <td style={{ fontWeight: 600 }}>{m.name}</td>
                  <td>{m.brandName}</td>
                  <td>{m.categoryName}</td>
                  <td>{m.year}</td>
                  <td style={{ textAlign: 'right' }}>€{m.price.toLocaleString()}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '.4rem', justifyContent: 'flex-end' }}>
                      <button onClick={() => editMoto(m)} className="btn btn-info btn-sm">Edit</button>
                      <button onClick={() => deleteMoto(m.id)} className="btn btn-danger btn-sm">Delete</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}

      {/* ─────── Brands ─────── */}
      {tab === 'brands' && (
        <>
          <div style={{ background: '#fff', border: '1px solid var(--border)', borderRadius: '8px', padding: '1.25rem', marginBottom: '1.5rem' }}>
            <h3 style={{ marginBottom: '1rem', fontSize: '1rem' }}>{editingId ? 'Edit' : 'Add'} Brand</h3>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 2fr', gap: '.75rem' }}>
              <input placeholder="Name *" value={brandForm.name} onChange={e => setBrandForm(f => ({ ...f, name: e.target.value }))} style={inputStyle} />
              <input placeholder="Country" value={brandForm.country} onChange={e => setBrandForm(f => ({ ...f, country: e.target.value }))} style={inputStyle} />
              <input placeholder="Logo URL" value={brandForm.logoUrl} onChange={e => setBrandForm(f => ({ ...f, logoUrl: e.target.value }))} style={inputStyle} />
            </div>
            <div style={{ display: 'flex', gap: '.5rem', marginTop: '.75rem' }}>
              <button onClick={saveBrand} className="btn btn-success">{editingId ? 'Update' : 'Add'} Brand</button>
              {editingId && <button onClick={cancelEdit} className="btn btn-ghost">Cancel</button>}
            </div>
          </div>

          <table className="table">
            <thead><tr><th>Name</th><th>Country</th><th></th></tr></thead>
            <tbody>
              {brands.map(b => (
                <tr key={b.id}>
                  <td style={{ fontWeight: 600 }}>{b.name}</td>
                  <td>{b.country ?? '—'}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '.4rem', justifyContent: 'flex-end' }}>
                      <button onClick={() => editBrand(b)} className="btn btn-info btn-sm">Edit</button>
                      <button onClick={() => deleteBrand(b.id)} className="btn btn-danger btn-sm">Delete</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}

      {/* ─────── Categories ─────── */}
      {tab === 'categories' && (
        <>
          <div style={{ background: '#fff', border: '1px solid var(--border)', borderRadius: '8px', padding: '1.25rem', marginBottom: '1.5rem' }}>
            <h3 style={{ marginBottom: '1rem', fontSize: '1rem' }}>{editingId ? 'Edit' : 'Add'} Category</h3>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 2fr', gap: '.75rem' }}>
              <input placeholder="Name *" value={categoryForm.name} onChange={e => setCategoryForm(f => ({ ...f, name: e.target.value }))} style={inputStyle} />
              <input placeholder="Description" value={categoryForm.description} onChange={e => setCategoryForm(f => ({ ...f, description: e.target.value }))} style={inputStyle} />
            </div>
            <div style={{ display: 'flex', gap: '.5rem', marginTop: '.75rem' }}>
              <button onClick={saveCategory} className="btn btn-success">{editingId ? 'Update' : 'Add'} Category</button>
              {editingId && <button onClick={cancelEdit} className="btn btn-ghost">Cancel</button>}
            </div>
          </div>

          <table className="table">
            <thead><tr><th>Name</th><th>Description</th><th></th></tr></thead>
            <tbody>
              {categories.map(c => (
                <tr key={c.id}>
                  <td style={{ fontWeight: 600 }}>{c.name}</td>
                  <td>{c.description ?? '—'}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '.4rem', justifyContent: 'flex-end' }}>
                      <button onClick={() => editCategory(c)} className="btn btn-info btn-sm">Edit</button>
                      <button onClick={() => deleteCategory(c.id)} className="btn btn-danger btn-sm">Delete</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}
    </div>
  );
};

export default AdminDashboard;
