# OneFitness — User Manual

OneFitness is a web portal for running a gym or fitness center: registering members, following up on enquiries, collecting payments and renewals, printing receipts, issuing refunds, and reporting on the business.

This manual walks through every screen in the order you would normally use them.

## Contents

1. [Signing in](#1-signing-in)
2. [Finding your way around](#2-finding-your-way-around)
3. [Dashboard](#3-dashboard)
4. [Members](#4-members)
5. [Enquiries](#5-enquiries)
6. [Enquiry Reasons](#6-enquiry-reasons)
7. [Payments and renewals](#7-payments-and-renewals)
8. [Refunds](#8-refunds)
9. [Generate Receipt](#9-generate-receipt)
10. [Setting up your plans](#10-setting-up-your-plans) — Workouts, Installments, Tax Master, Membership Types
11. [Reports](#11-reports)
12. [Role Master](#12-role-master)
13. [Users](#13-users)
14. [General Settings](#14-general-settings)
15. [First-time setup checklist](#15-first-time-setup-checklist)

---

## 1. Signing in

Open the portal in your browser and sign in with the username and password your administrator gave you. Use the eye icon to show or hide the password while typing.

![Login screen](screenshots/01-login.png)

If your session expires, you are taken back to this screen automatically. To sign out, click your name in the top-right corner and choose **Logout**.

> **Forgot your password?** Ask an administrator to reset it from the [Users](#13-users) screen.

## 2. Finding your way around

- **Sidebar (left):** every screen in the portal. The highlighted item is the screen you are on.
- **☰ button (top-left):** collapses the sidebar to icons, or opens it on a phone or tablet.
- **Your name (top-right):** shows your role and the **Logout** option.

### Who can see what

What you see depends on your role:

| Screen | Admin | Other roles (e.g. front desk) |
| --- | :---: | :---: |
| Dashboard, Members, Enquiries, Enquiry Reasons, Payments, Generate Receipt | ✅ | ✅ |
| Refunds, Membership Types, Installments, Workouts, Tax Master, Reports, Role Master, Users, Settings | ✅ | — |

Screens you don't have access to are hidden from the sidebar. If you open one directly, you'll see an **Access denied** message.

### Common actions on list screens

Most screens are a table of records with the same controls:

- **+ Add …** (top-right) opens a form to create a new record.
- **✎ Edit** and **🗑 Delete** icons appear in the **Actions** column. Deleting always asks you to confirm first.
- **Search box** filters the list as you type.
- **Previous / Next** at the bottom pages through long lists (10 rows per page).
- **Active** toggle: inactive records are kept for history but are not offered in drop-downs for new entries.

Required fields are checked when you click **Save**, and any problem is shown in red under the field.

## 3. Dashboard

Your home screen, with a snapshot of the business.

![Dashboard](screenshots/02-dashboard.png)

- **Top cards:** total members, active memberships, revenue collected this month, and new members this month.
- **Second row:** new members today, renewals this month, refunds this month, and total enquiries.
- **New vs Renewed:** a month-by-month chart for the current financial year (April to March).
- **Top Membership Types:** your most popular plans.
- **Recent Members:** the latest sign-ups and their current plan. Click **View all** to open the Members screen.

## 4. Members

Register and manage gym members. A member record holds personal and contact details only. Plans and money are handled on the [Payments](#7-payments-and-renewals) screen.

![Members list](screenshots/03-members.png)

Search by name, member number, mobile number or email. Each member gets a unique **Member No** (for example `OFV27516XTBCQCFP`) automatically.

### Adding a member

Click **+ Add Member**.

![Add Member form](screenshots/04-member-add.png)

- **Photo (optional):** click **Take Photo** to use the computer's webcam, or **Upload** to choose an image file.
- **Required:** First Name, Gender, Date of Birth, Joining Date and Address. **Age** fills in automatically from the date of birth.
- **Optional:** Middle/Last Name, Mobile No, Email, and emergency contact details.

Click **Save**. Next, go to **Payments** to put the new member on a plan.

### Membership history

Click the **🕘 history** icon on a member's row to see their current plan, next renewal date, and every invoice issued to them.

![Membership history](screenshots/04b-member-history.png)

From here you can **View / Print** any past invoice, or click **Record Payment** to go straight to the Payments screen for this member.

## 5. Enquiries

Record people who visit or call but haven't joined yet, so the team can follow up.

![Enquiries list](screenshots/05-enquiries.png)

Click **+ Add Enquiry** and enter the person's name, mobile number, email and gender. Then pick the **Workout** they are interested in and the **Reason** for the enquiry (see [Enquiry Reasons](#6-enquiry-reasons)), and add any notes in **Details**.

![Add Enquiry form](screenshots/06-enquiry-add.png)

When an enquiry has been dealt with, edit it and switch **Active** off. If the person joins, register them on the [Members](#4-members) screen.

## 6. Enquiry Reasons

The list of reasons that appears in the Enquiry form's **Reason** drop-down, for example *Walk-in enquiry*, *Phone call* or *Referral*.

![Enquiry Reasons](screenshots/07-enquiry-reasons.png)

Click **+ Add Reason**, type a name and click **Save**.

## 7. Payments and renewals

This is where a member is put on a plan and money is collected, both at joining and at each renewal.

![Payments screen](screenshots/08-payments.png)

### Step 1 — Select the member

Type a name, mobile number or member number in **Select Member** and choose the person from the list. Click **Change** to pick someone else.

![Payments with a member selected](screenshots/09-payment-member.png)

Once selected, the **Current Membership** panel shows their plan, next renewal date, total amount, amount paid, any **Balance Due**, and the current invoice number.

### Step 2a — Collect an outstanding balance

If the member owes money on their current plan, a **Collect balance** box appears. Enter the amount received (it cannot be more than the balance due) and click **Collect**.

### Step 2b — Record a new payment or renewal

In **Record Payment**:

1. Choose the **Workout** and the **Installment Plan** (e.g. Quarterly, Half Yearly). The **Membership Type** list then shows only plans that match both.
2. Choose the **Membership Type**, **Payment Type** (Cash, UPI, Card, …) and **Tax**.
3. Check the **Amount Summary**: plan amount, tax % and tax amount, and **Total Due**.
4. Enter the **Amount Collecting Now**. A partial amount is allowed, and whatever is left becomes the member's balance due.
5. Click **Record Payment**.

A confirmation shows the new invoice number and **next renewal date**, which the portal works out from the installment plan's number of months.

### Payment history

The table at the bottom lists every payment, newest first, with the total, the amount paid, any balance (in orange), and the next renewal date. Search by member name or number.

## 8. Refunds

*Admin only.* Issue and track refunds against a member's payment.

![Refunds list](screenshots/10-refunds.png)

Click **+ Add Refund**, search for the member and choose them. The form shows how much they have paid (the refundable amount) and their next renewal date. Enter the **Refund Amount** and click **Save**.

![Add Refund form](screenshots/11-refund-add.png)

Refunds appear in the Dashboard's *Refunds This Month* card and in the **Refund** and **Credit/Debit** reports.

## 9. Generate Receipt

View, print or reprint any invoice, current or past.

![Generate Receipt](screenshots/12-receipts.png)

**For one member:** pick the member under **Select Member**. You'll see their current plan's amount, tax, total and balance due, followed by a list of all their invoices.

![Receipts for a selected member](screenshots/12b-receipts-member.png)

- **Generate Receipt for Current Plan** creates a receipt for the member's current plan.
- **View / Print** on any invoice opens it.

**All receipts:** the **All Receipts** table lists every receipt issued. Search it by member number.

When a receipt is open, click **Print** to print it or save it as a PDF using your browser's print dialog. The receipt shows your company details from [General Settings](#14-general-settings), the member, the plan, the tax breakdown, and the totals:

![Printed receipt](screenshots/13-receipt.png)

## 10. Setting up your plans

*Admin only.* A **Membership Type** (the plan a member buys) combines a **Workout**, an **Installment** plan and a price. Set these up in the order below.

### Workouts

The programs your gym offers, for example *GYM*, *Yoga* or *Strength Training*. Enter a name and a short description.

![Workouts](screenshots/16-workouts.png)

### Installments

How long a plan lasts. Enter a name and a number of **Months**, for example *Quarterly = 3* or *Half Yearly = 6*. The number of months sets the member's next renewal date.

![Installments](screenshots/15-installments.png)

### Tax Master

The tax rates you charge, for example *GST 18%*, with an optional identification number such as your GSTIN. You choose the tax when recording each payment, and it appears on the receipt.

![Tax Master](screenshots/17-tax-master.png)

### Membership Types

The plans you sell. Give each one a **Name** (e.g. *Gold – 12 Months*) and an **Amount** (the price before tax), and pick its **Workout** and **Installment Plan**.

![Membership Types](screenshots/14-membership-types.png)

## 11. Reports

*Admin only.* Pick a report from the tabs, set the filters (financial year, month, or date range), and click **Generate**. Most reports have an **Export CSV** button that downloads the results for Excel.

![Reports](screenshots/18-reports.png)

| Report | What it shows | Filters |
| --- | --- | --- |
| **Yearwise** | Number of members who joined in each month of a financial year (Apr–Mar), with the year's total | Financial year |
| **Monthwise** | Members who joined in a given month, with amounts | Year, month |
| **Members Joined** | Everyone who joined in a period, with plan, amount and contact details | From / to date |
| **Renewal** | Renewals in a period and their next renewal dates | From / to date |
| **Refund** | Refunds issued in a period | From / to date |
| **Credit/Debit** | A ledger of receipts (credit) and refunds (debit), with totals and net | From / to date |
| **Tax Summary** | Taxable amount and tax collected, per tax type | From / to date |
| **Payment Type** | Collections split by Cash, UPI, Card, etc. | From / to date |
| **Staff Collection** | Receipts and refunds handled by each staff user | From / to date |
| **Renewals Due / Lapsed** | Members whose renewal date has passed (**Lapsed**) or falls within the next few days (**Due Soon**). Use it as a daily follow-up call list. | Due within (days) |
| **Outstanding Balances** | Members who still owe money, and the total outstanding | — |

**Export for Tally:** the **Credit/Debit** report also has an **Export for Tally** button that produces vouchers you can import into Tally accounting software. Test-import into a sample Tally company before using it with your live books.

<details>
<summary>Show a screenshot of every report</summary>

**Yearwise**
![Yearwise report](screenshots/18-report-01-yearwise.png)

**Monthwise**
![Monthwise report](screenshots/18-report-02-monthwise.png)

**Members Joined**
![Members Joined report](screenshots/18-report-03-members-joined.png)

**Renewal**
![Renewal report](screenshots/18-report-04-renewal.png)

**Refund**
![Refund report](screenshots/18-report-05-refund.png)

**Credit/Debit**
![Credit/Debit report](screenshots/18-report-06-credit-debit.png)

**Tax Summary**
![Tax Summary report](screenshots/18-report-07-tax-summary.png)

**Payment Type**
![Payment Type report](screenshots/18-report-08-payment-type.png)

**Staff Collection**
![Staff Collection report](screenshots/18-report-09-staff-collection.png)

**Renewals Due / Lapsed**
![Renewals Due / Lapsed report](screenshots/18-report-10-renewals-due-lapsed.png)

**Outstanding Balances**
![Outstanding Balances report](screenshots/18-report-11-outstanding-balances.png)

</details>

## 12. Role Master

*Admin only.* The job roles you can give to portal users, for example *Admin*, *Receptionist* or *Trainer*.

![Role Master](screenshots/19-roles.png)

The role named **Admin** has access to every screen. All other roles get the front-desk screens listed in [Who can see what](#who-can-see-what).

## 13. Users

*Admin only.* Staff accounts that can sign in to the portal.

![Users list](screenshots/20-users.png)

Click **+ Add User** and enter a username, name, email, mobile number and gender. Choose a **Role**, then set and confirm a password.

![Add User form](screenshots/21-user-add.png)

- **Reset password:** click the 🔒 lock icon on a user's row and enter a new password for them.
- **Disable an account:** edit the user and switch **Active** off. They will no longer be able to sign in.

## 14. General Settings

*Admin only.* Your company profile: company name, support email, mobile and telephone numbers, website, and address. These details appear on printed receipts.

![General Settings](screenshots/22-settings.png)
## 15. First-time setup checklist

Before the front desk starts using the portal, an administrator should:

1. **General Settings:** enter your company name, address and contact details.
2. **Workouts:** add the programs you offer.
3. **Installments:** add your plan durations (Monthly, Quarterly, …).
4. **Tax Master:** add your tax rate(s).
5. **Membership Types:** create the plans you sell, with prices.
6. **Enquiry Reasons:** add the ways people find you.
7. **Role Master and Users:** create roles and an account for each staff member.

After that, the day-to-day routine is:

**Enquiry → Add Member → Record Payment → Print Receipt**, then use **Reports → Renewals Due / Lapsed** to follow up on renewals.
