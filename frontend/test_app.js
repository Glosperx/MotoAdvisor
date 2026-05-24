const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

const SS_DIR = '/tmp/screenshots';
if (!fs.existsSync(SS_DIR)) fs.mkdirSync(SS_DIR);

const ss = async (page, name) => {
  const p = path.join(SS_DIR, `${name}.png`);
  await page.screenshot({ path: p, fullPage: true });
  console.log(`[screenshot] ${name} -> ${p}`);
};

(async () => {
  const browser = await chromium.launch({ headless: true });
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
  const page = await ctx.newPage();

  // Capture console errors
  page.on('console', msg => { if (msg.type() === 'error') console.log('[browser error]', msg.text()); });
  page.on('pageerror', err => console.log('[page error]', err.message));

  // 1. Homepage
  console.log('\n=== 1. Homepage ===');
  await page.goto('http://localhost:5173/', { waitUntil: 'networkidle' });
  await ss(page, '01_homepage');
  console.log('Title:', await page.title());
  console.log('H1/heading:', await page.locator('h1, h2').first().textContent().catch(() => 'none'));

  // 2. Register page
  console.log('\n=== 2. Register page ===');
  await page.goto('http://localhost:5173/register', { waitUntil: 'networkidle' });
  await ss(page, '02_register');

  // Try registering a test user
  const email = `test_${Date.now()}@example.com`;
  const username = `testuser_${Date.now()}`;
  try {
    await page.fill('input[name="username"], input[placeholder*="sername"], input[id*="username"]', username);
    await page.fill('input[type="email"], input[name="email"], input[placeholder*="mail"]', email);
    await page.fill('input[type="password"], input[name="password"], input[placeholder*="assword"]', 'TestPass123!');
    await ss(page, '03_register_filled');
    await page.click('button[type="submit"]');
    await page.waitForTimeout(2000);
    await ss(page, '04_register_result');
    console.log('URL after register:', page.url());
  } catch (e) {
    console.log('Register form error:', e.message);
    await ss(page, '03_register_error');
  }

  // 3. Login page
  console.log('\n=== 3. Login page ===');
  await page.goto('http://localhost:5173/login', { waitUntil: 'networkidle' });
  await ss(page, '05_login');
  try {
    await page.fill('input[type="email"], input[name="email"], input[placeholder*="mail"], input[name="username"]', email);
    await page.fill('input[type="password"]', 'TestPass123!');
    await ss(page, '06_login_filled');
    await page.click('button[type="submit"]');
    await page.waitForTimeout(2000);
    await ss(page, '07_login_result');
    console.log('URL after login:', page.url());
  } catch (e) {
    console.log('Login form error:', e.message);
    await ss(page, '06_login_error');
  }

  // 4. Motorcycle listing / home after login
  console.log('\n=== 4. Home after login ===');
  await page.goto('http://localhost:5173/', { waitUntil: 'networkidle' });
  await ss(page, '08_home_after_login');

  // 5. Click first motorcycle card if any
  console.log('\n=== 5. Motorcycle detail ===');
  try {
    const link = page.locator('a[href*="/motorcycle"], a[href*="/moto"]').first();
    if (await link.count() > 0) {
      await link.click();
      await page.waitForTimeout(1500);
      await ss(page, '09_motorcycle_detail');
      console.log('Moto detail URL:', page.url());
    } else {
      console.log('No motorcycle links found on homepage');
    }
  } catch (e) {
    console.log('Motorcycle detail error:', e.message);
  }

  // 6. Profile page
  console.log('\n=== 6. Profile ===');
  await page.goto('http://localhost:5173/profile', { waitUntil: 'networkidle' });
  await ss(page, '10_profile');
  console.log('Profile URL:', page.url());

  await browser.close();
  console.log('\nAll done. Screenshots in', SS_DIR);
})();
