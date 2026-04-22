# Employee Management System — Mini Project 2
## Full-Stack: .NET 8 Web API + SQL Server + Updated Frontend

**Batch:** Batch 7  
**Name:** Chinta Ramakrishna  

---

## Project Structure

```
EMS-MiniProject2/
├── EMS-MiniProject2.sln
├── EMS.API/                        ← .NET 8 Web API
│   ├── Controllers/
│   │   ├── EmployeesController.cs
│   │   └── AuthController.cs
│   ├── Data/
│   │   └── AppDbContext.cs         ← EF Core DbContext + seed data
│   ├── DTOs/
│   │   └── EmployeeDtos.cs
│   ├── Models/
│   │   ├── Employee.cs
│   │   └── AppUser.cs
│   ├── Services/
│   │   ├── IEmployeeRepository.cs
│   │   ├── EmployeeRepository.cs
│   │   ├── EmployeeService.cs
│   │   └── AuthService.cs
│   ├── Migrations/
│   ├── Program.cs
│   ├── appsettings.json
│   └── EMS.API.csproj
├── EMS.Tests/                      ← NUnit + Moq test project
│   ├── Services/
│   │   ├── EmployeeServiceTests.cs
│   │   └── AuthServiceTests.cs
│   ├── Integration/
│   │   └── EmployeeIntegrationTests.cs
│   └── EMS.Tests.csproj
└── frontend/                       ← Updated HTML/CSS/JS frontend
    ├── index.html
    ├── css/styles.css
    └── js/
        ├── config.js               ← NEW
        ├── authService.js          ← UPDATED
        ├── storageService.js       ← REPLACED
        ├── employeeService.js      ← UPDATED
        ├── validationService.js    ← UPDATED
        ├── dashboardService.js     ← UPDATED
        ├── uiService.js            ← UPDATED
        └── app.js                  ← UPDATED
```

---

## Prerequisites

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| SQL Server | 2022 (local) | Microsoft website — Express edition is free |
| dotnet-ef CLI | Latest | `dotnet tool install --global dotnet-ef` |
| VS Code + Live Server | Any | VS Code Marketplace |

---

## Step-by-Step Setup & Run

### Step 1 — Open Solution

Open `EMS-MiniProject2.sln` in Visual Studio 2022 (or VS Code).

---

### Step 2 — Restore NuGet Packages

In **Package Manager Console** or terminal inside `EMS.API/`:

```bash
dotnet restore
```

The following packages will be restored automatically from the `.csproj`:

- `Microsoft.EntityFrameworkCore` 8.0.0
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.0
- `Microsoft.EntityFrameworkCore.Tools` 8.0.0
- `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.0
- `Microsoft.IdentityModel.Tokens` 7.x
- `System.IdentityModel.Tokens.Jwt` 7.x
- `BCrypt.Net-Next` 4.0.3
- `Swashbuckle.AspNetCore` 6.5.0

---

### Step 3 — Configure Connection String

Open `EMS.API/appsettings.json` and verify your SQL Server connection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EMSDashboard1;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

For a **named SQL Server instance** (e.g., SQLEXPRESS), change to:
```
Server=localhost\SQLEXPRESS;Database=EMSDashboard;Trusted_Connection=True;TrustServerCertificate=True;
```

---

### Step 4 — Run EF Core Migrations

> ⚠️ **IMPORTANT: Do this BEFORE running the API for the first time.**

**Option A — Package Manager Console (Visual Studio):**
```
PM> Add-Migration InitialCreate
PM> Update-Database
```

**Option B — dotnet CLI (terminal inside `EMS.API/` folder):**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> A pre-built migration is already included in `EMS.API/Migrations/`.  
> If it already exists, just run `Update-Database` / `dotnet ef database update`.

This will:
- Create the `EMSDashboard` database on your SQL Server
- Create `Employees` and `AppUsers` tables
- Seed **15 employees** and **2 default user accounts**

---

### Step 5 — Run the API

**Visual Studio:** Press **F5** or **Ctrl+F5** with `EMS.API` as startup project.

**CLI:**
```bash
cd EMS.API
dotnet run
```

API runs at: **http://localhost:8080/api**
Swagger UI: **https://localhost:8081/swagger/index.html**

---

### Step 6 — Open the Frontend

Open `frontend/index.html` in one of these ways:

- **VS Code**: Right-click `index.html` → **Open with Live Server** (port 5500)
- **Direct**: Open `frontend/index.html` directly in Chrome

---

### Step 7 — Login

| Account | Username | Password | Role |
|---------|----------|----------|------|
| Admin   | `admin`  | `admin123`  | Full CRUD access |
| Viewer  | `viewer` | `viewer123` | Read-only access |

---

## Running Tests

### NUnit Tests (backend)

```bash
# Run all tests
dotnet test EMS.Tests

# With verbose output
dotnet test EMS.Tests --verbosity normal
```

Or in Visual Studio: **Test Explorer → Run All Tests**

Test coverage:
- `EmployeeServiceTests` — Unit tests with Moq (GetById, Create, Update, Delete, GetAll)
- `AuthServiceTests` — Login, Register, JWT token, BCrypt, case-insensitive login
- `EmployeeIntegrationTests` — EF Core InMemory (Add/Retrieve, Delete, Email uniqueness, Dashboard counts, Pagination, Filters)

---

## API Endpoints Reference

### Authentication (no token required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new account |
| POST | `/api/auth/login` | Login — returns JWT token |

### Employees (JWT token required)
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| GET | `/api/employees` | Admin + Viewer | Paged list with filters |
| GET | `/api/employees/{id}` | Admin + Viewer | Get by ID |
| GET | `/api/employees/dashboard` | Admin + Viewer | Dashboard KPIs + breakdown |
| POST | `/api/employees` | Admin only | Create employee |
| PUT | `/api/employees/{id}` | Admin only | Update employee |
| DELETE | `/api/employees/{id}` | Admin only | Delete employee |

### Query Parameters for GET /api/employees
| Param | Default | Description |
|-------|---------|-------------|
| `search` | — | Name or email search (LIKE) |
| `department` | — | Exact filter |
| `status` | — | Active / Inactive |
| `sortBy` | `name` | name / salary / joinDate |
| `sortDir` | `asc` | asc / desc |
| `page` | `1` | Page number (1-based) |
| `pageSize` | `10` | Records per page (max 100) |

---

## Default Credentials Summary

```
admin  / admin123  → Role: Admin  (can Add, Edit, Delete, View)
viewer / viewer123 → Role: Viewer (can only View)
```

---

## Architecture Note

The **storageService.js boundary** from Mini Project 1 is what makes this migration possible:

- `employeeService.js`, `dashboardService.js`, `validationService.js`, `uiService.js` — **zero architectural changes**
- Only `storageService.js` (data layer) and `authService.js` (auth layer) changed implementations
- All Mini Project 1 business logic moved cleanly to the backend

---

## Examiner Checklist

1. Open solution in Visual Studio 2022
2. Run `Update-Database` in Package Manager Console
3. Press F5 to start the API
4. Verify Swagger at https://localhost:8081/swagger/index.html
5. Open `frontend/index.html` with Live Server
6. Login as `admin / admin123` — test CRUD operations
7. Login as `viewer / viewer123` — verify read-only mode
8. Run `dotnet test EMS.Tests` — all tests should pass
