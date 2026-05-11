# SQL Server Quick Reference

## ? Quick Setup (2 Minutes)

### Step 1: Update Connection String (Already Done)
? `appsettings.json` is configured for SQL Server

### Step 2: Verify SQL Server Running
Open **SQL Server Management Studio** and connect to `localhost`

### Step 3: Run Migrations
```bash
cd TaskManagement.API
dotnet ef database update
```

### Step 4: Start Application
```bash
dotnet run
```

---

## ?? Common Connection Strings

### Windows Authentication (Recommended for Development)
```
Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;
```

### SQL Server Authentication
```
Server=localhost;Database=TaskManagement;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;
```

### Named Instance (SQL Server Express)
```
Server=localhost\SQLEXPRESS;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;
```

### Azure SQL Database
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TaskManagement;Persist Security Info=False;User ID=your-username;Password=your-password;Encrypt=True;TrustServerCertificate=False;
```

---

## ?? Common Tasks

### Create SQL Server Login
```sql
CREATE LOGIN taskmanagement_user WITH PASSWORD = 'Password123!';
USE TaskManagement;
CREATE USER taskmanagement_user FOR LOGIN taskmanagement_user;
ALTER ROLE db_owner ADD MEMBER taskmanagement_user;
```

### Create New Migration
```bash
cd TaskManagement.Infrastructure
dotnet ef migrations add MigrationName -p ../TaskManagement.API
```

### Update Database
```bash
cd TaskManagement.API
dotnet ef database update
```

### Rollback to Previous Migration
```bash
cd TaskManagement.API
dotnet ef database update PreviousMigrationName
```

### View Migration History
```bash
cd TaskManagement.API
dotnet ef migrations list
```

---

## ?? Verify Installation

### Check Database Exists
```sql
SELECT name FROM sys.databases WHERE name = 'TaskManagement';
```

### List All Tables
```sql
USE TaskManagement;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
```

### View Table Structure
```sql
USE TaskManagement;
EXEC sp_help 'Tasks';
```

---

## ?? Troubleshooting

| Issue | Solution |
|-------|----------|
| Cannot connect | Verify SQL Server is running (Services > SQL Server) |
| Login failed | Check username/password in connection string |
| Instance not found | Use `localhost\SQLEXPRESS` for SQL Server Express |
| Port 1433 unavailable | Check if SQL Server is running, restart if needed |
| Database missing | Run `dotnet ef database update` |

---

## ?? Full Documentation

For complete setup and configuration details, see: **[SQL_SERVER_SETUP_GUIDE.md](./SQL_SERVER_SETUP_GUIDE.md)**

