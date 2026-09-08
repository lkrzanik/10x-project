import { expect, Page, test } from '@playwright/test';

const adminEmail = process.env.AdminEmail;
const adminPassword = process.env.AdminPassword;

async function signIn(page: Page) {
  await page.goto('/Account/Login');
  await page.getByLabel('Email').fill(adminEmail ?? '');
  await page.getByLabel('Hasło').fill(adminPassword ?? '');
  await page.getByRole('button', { name: 'Zaloguj' }).click();
  await expect(page.getByRole('button', { name: 'Wyloguj' })).toBeVisible();
}

test('M2-3: correlation view exposes accessible filters and result state', async ({ page }) => {
  await signIn(page);

  await page.goto('/ClimateCorrelation/Correlation');

  await expect(page).toHaveURL(/\/ClimateData\/Correlation/);
  await expect(page.getByRole('heading', { name: 'Korelacja danych klimatycznych' })).toBeVisible();
  await expect(page.getByLabel('Od')).toBeVisible();
  await expect(page.getByLabel('Do')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Oblicz korelację' })).toBeVisible();

  const resultTable = page.getByRole('table', { name: 'Skorelowane wartości temperatury dla czujnika i pogody' });
  const emptyState = page.getByText('Brak danych do wyświetlenia.');

  if (await resultTable.count() > 0) {
    await expect(resultTable).toBeVisible();
    await expect(page.getByRole('img', { name: 'Wykres temperatury czujnika i pogody' })).toBeVisible();
    await expect(page.getByText('Czujnik', { exact: true })).toBeVisible();
    await expect(page.getByText('Pogoda', { exact: true })).toBeVisible();
  } else {
    await expect(emptyState).toBeVisible();
  }
});