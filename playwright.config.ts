import { defineConfig } from '@playwright/test';
import dotenv from 'dotenv';

dotenv.config({ quiet: true });

const baseUrl = process.env.AppUrl;
const adminEmail = process.env.AdminEmail;
const adminPassword = process.env.AdminPassword;

if (!adminEmail || !adminPassword) {
  throw new Error('AdminEmail and AdminPassword must be set in environment variables or .env file.');
}

export default defineConfig({
  testDir: './app/tests/e2e',
  fullyParallel: false,
  retries: 0,
  timeout: 60_000,
  use: {
    baseURL: baseUrl,
    trace: 'on-first-retry'
  },
  webServer: {
    command: 'dotnet run --project app/10xPV.csproj',
    url: baseUrl,
    ignoreHTTPSErrors: true,
    timeout: 120_000,
    reuseExistingServer: true,
    env: {
      ASPNETCORE_ENVIRONMENT: 'Development',
      AdminEmail: adminEmail,
      AdminPassword: adminPassword
    }
  }
});
