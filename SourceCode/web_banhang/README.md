# Docker Compose Demo

This folder contains the Docker Compose setup for running the backend and frontend together.

## Preparation

1. Copy `.env.example` to `.env`:

```powershell
copy .env.example .env
```

2. Edit `.env` with Docker values:
   - `POSTGRES_PASSWORD`

3. Edit `../web_banhang_be/.env` with backend values:
   - `JwtSettings__SecretKey`
   - `Supabase__Url` / `Supabase__ServiceRoleKey` for image upload
   - `DemoSeed__Enabled`
   - `Authentication__Google__ClientId` / `Authentication__Google__ClientSecret` if needed

## Environment files

- `web_banhang/.env`: used by Docker Compose for ports and PostgreSQL container values.
- `web_banhang_be/.env`: loaded into the API container by `docker-compose.yml`.
- `web_banhang/.env.prod`: used with `docker-compose.prod.yml` for a production-like stack.

Only commit `.env.example` files. Do not commit real `.env` files.

## Run locally

```powershell
docker compose up --build
```

## Run production/demo locally

1. Copy `.env.prod.example` to `.env.prod`:

```powershell
copy .env.prod.example .env.prod
```

2. Start the production stack:

```powershell
docker compose -f docker-compose.prod.yml --env-file .env.prod up --build
```

## Access

- Dev FE: `http://localhost:4200`
- Dev API: `http://localhost:7100/api/v1`
- PostgreSQL: `localhost:5432`

## Production deployment target

Recommended low-cost demo stack:

- Frontend: Vercel
- Backend: Render
- Database: Supabase PostgreSQL
- Images: Supabase Storage

Production environment values:

```text
ConnectionStrings__PostgresConnection=<Supabase PostgreSQL connection string>
Supabase__Url=https://<project-ref>.supabase.co
Supabase__ServiceRoleKey=<service-role-key>
Supabase__StorageBucket=product-images
AllowedOrigins=https://www.your-domain.com,https://your-domain.com
JwtSettings__Issuer=https://api.your-domain.com
JwtSettings__Audience=https://www.your-domain.com
```

Frontend production API URL is configured in `web_banhang_fe/src/environments/environment.prod.ts`.
Change `https://api.your-domain.com/api/v1` to your real API domain before deploying.

## Demo deployment advice

If you want to use this project as a CV/demo without requiring HR to pull code and run it locally,
then deploy it to a public URL.

- Option 1: Deploy the frontend to Vercel and backend to Render.
- Option 2: Deploy both frontend and backend together using Docker on a cloud VM.
- Option 3: Deploy the backend to Railway/Render and keep PostgreSQL + Storage on Supabase.

Then put the public link in your CV, so HR can open the demo directly.

## Important

- `.env` should not be committed.
- `.env.example` is the sample file for others to copy.
