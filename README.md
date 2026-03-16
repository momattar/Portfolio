# Mohamed Ashraf Mattar — Portfolio

<div align="center">

[![Live](https://img.shields.io/badge/Live-themomattar.runasp.net-003df4?style=for-the-badge&logo=google-chrome&logoColor=white)](https://themomattar.runasp.net)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)

</div>

---

## Overview

A full-stack personal portfolio built with ASP.NET Core MVC, Entity Framework Core, and SQL Server. All content is database-driven and managed through a hidden admin panel — no hardcoded data anywhere in the views.

**Live at:** [themomattar.runasp.net](https://themomattar.runasp.net)

---

## Features

**Frontend**
- Single-page layout with smooth scroll navigation
- Dark / light theme toggle persisted in localStorage
- Scroll-triggered reveal animations
- Fully responsive — mobile, tablet, desktop

**Admin Panel**
- Accessible via hidden route `/admin` — no visible trigger on the page
- Password verified server-side with SHA-256 hashing
- Rate-limited login (max 5 attempts per 15 minutes) via ASP.NET Core rate limiter
- Session-based authentication with secure HTTP-only cookies
- Full CRUD for: Projects, Skills, Education, Experience, Certifications, CV URL

**Content Sections**
- Hero with dynamic project count stat
- About cards
- Tech stack (icon-mapped from DB)
- Experience timeline
- Projects grid with image upload (Base64, stored in DB)
- Education & certifications
- Contact form (saved to DB)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core MVC, C# |
| ORM | Entity Framework Core 9 |
| Database | SQL Server |
| Auth | Session-based, SHA-256, Rate Limiting |
| Frontend | HTML, CSS, JavaScript (Vanilla) |
| Fonts | Syne, Epilogue (Google Fonts) |
| Icons | Font Awesome 6 |

---

## Project Structure

```
Portfolio/
├── Controllers/
│   └── HomeController.cs       # All MVC actions + admin CRUD endpoints
├── Data/
│   └── AppDbContext.cs          # EF Core DbContext
│   └── AppDbContextFactory.cs   # Design-time factory for migrations
├── Models/
│   ├── Project.cs
│   ├── Skill.cs
│   ├── Education.cs
│   ├── Experience.cs
│   ├── Certificate.cs
│   └── Contact.cs
├── Views/
│   └── Home/
│       └── Index.cshtml         # Single-page SPA view
├── wwwroot/
│   └── images/                  # Static assets
├── appsettings.json             # Connection string + admin password hash
└── Program.cs                   # Service registration, middleware, routing
```

---

## Security

- Admin password is **never stored in plain text** — SHA-256 hashed and stored in `appsettings.json`
- Login endpoint is **rate limited** — 5 attempts per 15 minutes per IP
- All write actions check session authentication server-side before executing
- Session cookie is `HttpOnly`, `SameSite=Strict`
- Admin panel URL is hidden — no link or button exposed on the public page

---

## Built by

**Mohamed Ashraf Mattar** — .NET Software Engineer  
[LinkedIn](https://linkedin.com/in/momattar) · [Portfolio](https://themomattar.runasp.net)
