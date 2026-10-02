# OneFitness

A web portal for running a gym or fitness center: members, enquiries, payments and renewals, receipts, refunds and business reports, all in one dashboard.

![OneFitness dashboard](OneFitnessUI/docs/screenshots/02-dashboard.png)

📖 **[Read the User Manual](OneFitnessUI/docs/USER_MANUAL.md)** for a screen-by-screen guide.

## Features

- **Members:** registration with webcam or uploaded photo, search, and full membership and invoice history
- **Enquiries:** capture walk-ins and calls, with configurable enquiry reasons
- **Payments and renewals:** plan selection by workout and installment, tax calculation, partial payments with balance tracking, and automatic next-renewal dates
- **Receipts:** view, print or reprint any invoice
- **Refunds:** issue and track refunds against a payment
- **Reports:** 11 reports, including renewals due/lapsed, outstanding balances, tax summary and staff collection, with CSV export and Tally export
- **Administration:** workouts, installment plans, tax rates, membership types, roles, users and company settings
- **Role-based access:** Admins see everything; front-desk roles see day-to-day screens only

## Screenshots

| | |
| :---: | :---: |
| ![Members](OneFitnessUI/docs/screenshots/03-members.png)<br>**Members** | ![Add Member](OneFitnessUI/docs/screenshots/04-member-add.png)<br>**Add Member** |
| ![Payments](OneFitnessUI/docs/screenshots/09-payment-member.png)<br>**Payments & renewals** | ![Receipt](OneFitnessUI/docs/screenshots/13-receipt.png)<br>**Printable receipt** |
| ![Enquiries](OneFitnessUI/docs/screenshots/05-enquiries.png)<br>**Enquiries** | ![Refunds](OneFitnessUI/docs/screenshots/10-refunds.png)<br>**Refunds** |
| ![Reports](OneFitnessUI/docs/screenshots/18-report-10-renewals-due-lapsed.png)<br>**Reports — Renewals Due / Lapsed** | ![Membership Types](OneFitnessUI/docs/screenshots/14-membership-types.png)<br>**Membership Types** |
| ![Users](OneFitnessUI/docs/screenshots/20-users.png)<br>**Users** | ![Login](OneFitnessUI/docs/screenshots/01-login.png)<br>**Sign in** |

More screenshots of every screen and report are in the [User Manual](OneFitnessUI/docs/USER_MANUAL.md).

## Tech stack

| Part | Folder | Technology |
| --- | --- | --- |
| Frontend | [`OneFitnessUI`](OneFitnessUI) | Next.js 16, React 19, TypeScript, Tailwind CSS 4 |
| Backend API | [`OneFitnessAPIs`](OneFitnessAPIs) | ASP.NET Core Web API, Entity Framework Core, JWT authentication |
| Database | — | SQL Server (default) or PostgreSQL |

## Running locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) and the EF Core tools (`dotnet tool install --global dotnet-ef`)
- [Node.js](https://nodejs.org/) 20 or newer
- SQL Server (or PostgreSQL)

### 1. Start the API

```bash
cd OneFitnessAPIs
cp OneFitness.API/appsettings.Development.example.json OneFitness.API/appsettings.Development.json
```

Edit `OneFitness.API/appsettings.Development.json` and set `Jwt:Key` to a long random secret (64+ characters). This file is git-ignored, so keep secrets here, not in `appsettings.json`.

Check the connection string and `DatabaseProvider` (`SqlServer` or `PostgreSql`) in `appsettings.json`.

Create the database by applying the migrations (use `PostgreSqlApplicationDbContext` for PostgreSQL):

```bash
dotnet ef database update --project OneFitness.Repository --startup-project OneFitness.API --context SqlServerApplicationDbContext
```

Then start the API:

```bash
cd OneFitness.API
dotnet run
```

The API listens on `http://localhost:5284`.

> No login account is created automatically. Add the first **Admin** user to the database before signing in. After that, further users can be created from the **Users** screen.

### 2. Start the UI

```bash
cd OneFitnessUI
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) and sign in.

To point the UI at a different API, set `NEXT_PUBLIC_API_BASE_URL` in `OneFitnessUI/.env.local`:

```bash
NEXT_PUBLIC_API_BASE_URL=http://localhost:5284/api
```
