# SQL Server Setup Guide for Task Management API

This guide provides step-by-step instructions to configure the Task Management API to use **SQL Server** with **Entity Framework Core**.

---

## ?? Prerequisites

Before you begin, ensure you have the following installed:

### 1. SQL Server
- **SQL Server 2019** or later (Express, Developer, or Standard Edition)
- Download: [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)
- Or: [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### 2. SQL Server Management Studio (SSMS)
- Download: [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Used for managing databases and executing queries

### 3. .NET 8 SDK
- Already installed (for this project)

### 4. Visual Studio 2022 (Optional but Recommended)
- For running migrations and managing the project

---

## ?? Quick Setup (Windows Authentication)

### Step 1: Verify SQL Server Installation

Open **SQL Server Management Studio (SSMS)** and connect to your SQL Server instance:

```
Server name: localhost
Authentication: Windows Authentication
```

If you can connect successfully, SQL Server is ready.

### Step 2: Review Connection String Configuration

The project is already configured to use SQL Server. Check the current configuration in:

**File:** `TaskManagement.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Connection String Breakdown:**
- `Server=localhost` - SQL Server instance running locally
- `Database=TaskManagement` - Database name (will be created automatically)
- `Trusted_Connection=true` - Uses Windows Authentication
- `TrustServerCertificate=true` - Trusts the server certificate (important for local development)

### Step 3: Update Connection String (If Needed)

If your SQL Server instance is **not** named `localhost`, update the connection string:

**Example for Named Instance:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;"
```

**Example for Different Server:**
```json
"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;"
```

### Step 4: Run Entity Framework Migrations

Navigate to the API project directory and run migrations:

```bash
cd TaskManagement.API
dotnet ef database update
```

**Expected Output:**
```
Build started...
Build succeeded.
Done. 2 migrations applied. Database updated successfully.
```

The database will be created automatically with all required tables.

### Step 5: Start the Application

```bash
dotnet run
```

The application will start and connect to SQL Server.

---

## ?? Setup with SQL Server Authentication (Username/Password)

If you prefer to use **SQL Server Authentication** instead of Windows Authentication, follow these steps:

### Step 1: Create a SQL Server Login

Open **SQL Server Management Studio (SSMS)** and execute:

```sql
-- Create a login for the application
CREATE LOGIN taskmanagement_user WITH PASSWORD = 'YourSecurePassword123!';

-- Create the database
CREATE DATABASE TaskManagement;

-- Create a user in the database
USE TaskManagement;
CREATE USER taskmanagement_user FOR LOGIN taskmanagement_user;

-- Grant database owner permissions
ALTER ROLE db_owner ADD MEMBER taskmanagement_user;
```

### Step 2: Update Connection String

**File:** `TaskManagement.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagement;User Id=taskmanagement_user;Password=YourSecurePassword123!;TrustServerCertificate=true;"
  }
}
```

**Connection String Breakdown:**
- `Server=localhost` - SQL Server instance
- `Database=TaskManagement` - Database name
- `User Id=taskmanagement_user` - SQL Server login
- `Password=YourSecurePassword123!` - Login password
- `TrustServerCertificate=true` - Trust the certificate

### Step 3: Run Migrations

```bash
cd TaskManagement.API
dotnet ef database update
```

### Step 4: Start the Application

```bash
dotnet run
```

---

## ?? Production Environment Setup (appsettings.Production.json)

For production deployments, create a separate configuration file:

**File:** `TaskManagement.API/appsettings.Production.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_PRODUCTION_SERVER;Database=TaskManagement;User Id=YOUR_PROD_USER;Password=YOUR_SECURE_PASSWORD;TrustServerCertificate=true;Encrypt=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  }
}
```

**Additional Security Recommendations:**
- Use a strong password (mix of uppercase, lowercase, numbers, special characters)
- Store passwords in **Azure Key Vault** or **AWS Secrets Manager**
- Use environment variables for sensitive data:

```csharp
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
```

---

## ??? Common Connection String Formats

### Local SQL Server (Windows Auth)
```
Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;
```

### SQL Server Express (Windows Auth)
```
Server=localhost\SQLEXPRESS;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;
```

### SQL Server with Username/Password
```
Server=localhost;Database=TaskManagement;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;
```

### Azure SQL Database
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TaskManagement;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### AWS RDS for SQL Server
```
Server=your-rds-endpoint.amazonaws.com;Database=TaskManagement;User Id=admin;Password=your-password;TrustServerCertificate=true;
```

---

## ?? Verify Database Creation

### Using SQL Server Management Studio (SSMS)

1. Open **SQL Server Management Studio**
2. Connect to your SQL Server instance
3. Expand **Databases** in the Object Explorer
4. Look for **TaskManagement** database
5. Expand it to see the tables:
   - `Tasks` - Main tasks table
   - `__EFMigrationsHistory` - Entity Framework migration history

### Using SQL Query

```sql
SELECT name FROM sys.databases WHERE name = 'TaskManagement';
```

**Expected Output:**
```
name
----
TaskManagement
```

### View Tables

```sql
USE TaskManagement;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
```

---

## ?? Running Migrations

### Create a New Migration

If you've made changes to the `DbContext` or entities, create a new migration:

```bash
cd TaskManagement.Infrastructure
dotnet ef migrations add MigrationName -p ../TaskManagement.API
```

### Update Database

```bash
cd TaskManagement.API
dotnet ef database update
```

### Rollback Last Migration

```bash
cd TaskManagement.API
dotnet ef database update PreviousMigration
```

### Remove Last Migration

```bash
cd TaskManagement.Infrastructure
dotnet ef migrations remove -p ../TaskManagement.API
```

---

## ?? Troubleshooting

### Error: "Cannot connect to server"

**Cause:** SQL Server is not running or incorrect server name

**Solution:**
1. Verify SQL Server is running: Open Services (services.msc) and check SQL Server status
2. Check server name in connection string
3. Use `localhost` or `localhost\SQLEXPRESS` for local instances

---

### Error: "Login failed for user"

**Cause:** Incorrect username or password

**Solution:**
1. Verify the credentials in the connection string
2. Ensure the user has been created in SQL Server
3. Check password complexity requirements

---

### Error: "Database already exists"

**Cause:** Database was already created

**Solution:**
1. This is not an error - the migration will skip existing tables
2. Run `dotnet ef database update` to apply any pending migrations

---

### Error: "The provider for connection string 'DefaultConnection' could not be loaded"

**Cause:** Missing or incorrect NuGet package

**Solution:**
Verify `Microsoft.EntityFrameworkCore.SqlServer` is installed:

```bash
cd TaskManagement.Infrastructure
dotnet list package
```

Install if missing:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

---

### Error: "A network-related or instance-specific error occurred"

**Cause:** Network connectivity issue or SQL Server not accessible

**Solution:**
1. Verify SQL Server is running
2. Check firewall settings (SQL Server uses port 1433)
3. Ensure the server name is correct
4. Try pinging the server if remote

---

## ?? Performance Tuning for SQL Server

### Enable Connection Pooling

```json
"DefaultConnection": "Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;Min Pool Size=5;Max Pool Size=100;"
```

### Connection String Parameters

| Parameter | Purpose | Default |
|-----------|---------|---------|
| `Min Pool Size` | Minimum connections to keep open | 5 |
| `Max Pool Size` | Maximum connections | 100 |
| `Connection Timeout` | Seconds to wait for connection | 15 |
| `Command Timeout` | Seconds to wait for command | 30 |

### Example Production Connection String

```json
"DefaultConnection": "Server=your-server;Database=TaskManagement;User Id=app-user;Password=SecurePassword123!;TrustServerCertificate=true;Encrypt=true;Min Pool Size=10;Max Pool Size=100;Connection Timeout=30;Command Timeout=60;"
```

---

## ?? Verify Configuration

### Check Current Connection String

Add this to your `Program.cs` for debugging:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using connection string: {connectionString}");
```

### View EF Core Configuration

Enable detailed logging to see SQL Server interactions:

**File:** `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
    }
  }
}
```

This will log all SQL queries executed by Entity Framework.

---

## ? Verification Checklist

- [ ] SQL Server is installed and running
- [ ] SSMS connects successfully to the instance
- [ ] Connection string is updated in `appsettings.json`
- [ ] `Microsoft.EntityFrameworkCore.SqlServer` NuGet package is installed
- [ ] `Program.cs` uses `UseSqlServer()` instead of `UseSqlite()`
- [ ] Migrations are applied: `dotnet ef database update`
- [ ] Database exists in SQL Server
- [ ] Tables are created in the database
- [ ] Application starts without connection errors
- [ ] API endpoints work with SQL Server backend

---

## ?? Additional Resources

- [Entity Framework Core - SQL Server Provider](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)
- [SQL Server Connection Strings](https://www.connectionstrings.com/sql-server/)
- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [SQL Server Security Best Practices](https://learn.microsoft.com/en-us/sql/relational-databases/security/sql-server-security-best-practices)

---

## ?? Next Steps

Once SQL Server is configured:

1. **Run the application**: `dotnet run`
2. **Test API endpoints**: Use the API documentation at [README_API_DOCUMENTATION.md](./README_API_DOCUMENTATION.md)
3. **Monitor database**: Use SSMS to view tables and data
4. **Setup backups**: Configure automated SQL Server backups for production

---

**Built with ?? using .NET 8 and SQL Server**

