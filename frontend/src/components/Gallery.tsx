import { useCallback, useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';

interface GalleryProps {
  images: string[];
  alt: string;
}

const Gallery = ({ images, alt }: GalleryProps) => {
  const [active, setActive]       = useState(0);
  const [animKey, setAnimKey]     = useState(0);
  const [lightbox, setLightbox]   = useState(false);
  const [lbAnimKey, setLbAnimKey] = useState(0);
  const thumbsRef = useRef<HTMLDivElement>(null);

  // Navigate gallery (and lightbox when open)
  const go = useCallback((next: number) => {
    const idx = (next + images.length) % images.length;
    setActive(idx);
    setAnimKey(k => k + 1);
    setLbAnimKey(k => k + 1);
  }, [images.length]);

  const prev = useCallback(() => go(active - 1), [active, go]);
  const next = useCallback(() => go(active + 1), [active, go]);

  const openLightbox  = () => setLightbox(true);
  const closeLightbox = () => setLightbox(false);

  // Global keyboard: arrows navigate, ESC closes lightbox
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') { closeLightbox(); return; }
      const tag = (e.target as HTMLElement).tagName;
      if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;
      if (e.key === 'ArrowLeft')  { e.preventDefault(); prev(); }
      if (e.key === 'ArrowRight') { e.preventDefault(); next(); }
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [prev, next]);

  // Lock body scroll while lightbox is open
  useEffect(() => {
    document.body.style.overflow = lightbox ? 'hidden' : '';
    return () => { document.body.style.overflow = ''; };
  }, [lightbox]);

  // Scroll active thumbnail into view
  useEffect(() => {
    const strip = thumbsRef.current;
    if (!strip) return;
    const thumb = strip.children[active] as HTMLElement | undefined;
    thumb?.scrollIntoView({ inline: 'center', behavior: 'smooth', block: 'nearest' });
  }, [active]);

  if (images.length === 0) {
    return (
      <div className="gallery-main gallery-placeholder-wrap">
        <div className="gallery-placeholder">🏍️</div>
      </div>
    );
  }

  return (
    <>
      <div className="gallery">
        {/* ── Main image ── */}
        <div className="gallery-main">
          <img
            key={animKey}
            src={images[active]}
            alt={`${alt} — photo ${active + 1}`}
            className="gallery-img gallery-img-clickable"
            onClick={openLightbox}
            title="Click to enlarge"
          />

          {images.length > 1 && (
            <>
              <button className="gallery-arrow gallery-arrow-left"  onClick={e => { e.stopPropagation(); prev(); }} aria-label="Previous image">‹</button>
              <button className="gallery-arrow gallery-arrow-right" onClick={e => { e.stopPropagation(); next(); }} aria-label="Next image">›</button>
            </>
          )}

          <div className="gallery-counter">
            {active + 1} <span>/</span> {images.length}
            {images.length > 1 && <span className="gallery-counter-hint"> · click to enlarge</span>}
          </div>
        </div>

        {/* ── Thumbnail strip ── */}
        {images.length > 1 && (
          <div className="gallery-thumbs" ref={thumbsRef}>
            {images.map((url, i) => (
              <button
                key={i}
                className={`gallery-thumb ${active === i ? 'active' : ''}`}
                onClick={() => { setActive(i); setAnimKey(k => k + 1); setLbAnimKey(k => k + 1); }}
                aria-label={`View photo ${i + 1}`}
                aria-pressed={active === i}
              >
                <img src={url} alt="" draggable={false} />
              </button>
            ))}
          </div>
        )}
      </div>

      {/* ── Lightbox (rendered in portal so it sits above everything) ── */}
      {lightbox && createPortal(
        <div
          className="lb-overlay"
          onClick={closeLightbox}
          role="dialog"
          aria-modal="true"
          aria-label={`${alt} — photo ${active + 1} of ${images.length}`}
        >
          {/* Close button */}
          <button className="lb-close" onClick={closeLightbox} aria-label="Close lightbox">✕</button>

          {/* Counter */}
          <div className="lb-counter">{active + 1} <span>/</span> {images.length}</div>

          {/* Main image — stop propagation so clicks on the image don't close */}
          <div className="lb-img-wrap" onClick={e => e.stopPropagation()}>
            <img
              key={lbAnimKey}
              src={images[active]}
              alt={`${alt} — photo ${active + 1}`}
              className="lb-img"
            />
          </div>

          {/* Arrows */}
          {images.length > 1 && (
            <>
              <button
                className="lb-arrow lb-arrow-left"
                onClick={e => { e.stopPropagation(); prev(); }}
                aria-label="Previous image"
              >‹</button>
              <button
                className="lb-arrow lb-arrow-right"
                onClick={e => { e.stopPropagation(); next(); }}
                aria-label="Next image"
              >›</button>
            </>
          )}

          {/* Thumbnail strip inside lightbox */}
          {images.length > 1 && (
            <div className="lb-thumbs" onClick={e => e.stopPropagation()}>
              {images.map((url, i) => (
                <button
                  key={i}
                  className={`lb-thumb ${active === i ? 'active' : ''}`}
                  onClick={() => { setActive(i); setLbAnimKey(k => k + 1); setAnimKey(k => k + 1); }}
                  aria-label={`View photo ${i + 1}`}
                >
                  <img src={url} alt="" draggable={false} />
                </button>
              ))}
            </div>
          )}
        </div>,
        document.body
      )}
    </>
  );
};

export default Gallery;
