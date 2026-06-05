import { expect, test } from '@playwright/test';

const adminEmail = process.env.AdminEmail ?? 'admin@10xpv.local';
const adminPassword = process.env.AdminPassword ?? 'Dev123!';

test('R7: login gates protected route and grants access after valid sign-in', async ({ page }) => {
  await page.goto('/');

  await expect(page).toHaveURL(/\/Account\/Login/);
  await expect(page.getByRole('heading', { name: 'Logowanie' })).toBeVisible();

  await page.getByLabel('Email').fill(adminEmail);
  await page.getByLabel('Hasło').fill(adminPassword);
  await page.getByRole('button', { name: 'Zaloguj' }).click();

  await expect(page).toHaveURL(/\/$|\/Home\/Index/);
  await expect(page.getByRole('heading', { name: 'Witaj w 10xPV' })).toBeVisible();
  await expect(page.getByRole('button', { name: 'Wyloguj' })).toBeVisible();
});
