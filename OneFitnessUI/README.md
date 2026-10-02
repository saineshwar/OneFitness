# OneFitness UI

The Next.js frontend for OneFitness. See the [main README](../README.md) for the project overview and full setup, and the [User Manual](docs/USER_MANUAL.md) for a screen-by-screen guide.

```bash
npm install
npm run dev     # http://localhost:3000
```

The UI talks to the OneFitness API at `http://localhost:5284/api` by default. Override it with `NEXT_PUBLIC_API_BASE_URL` in `.env.local`.

Other scripts: `npm run build`, `npm start`, `npm run lint`.
