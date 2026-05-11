# SQL Server Configuration - Complete Reference

## ?? What Changed?

Your Task Management API has been successfully configured to use **SQL Server** instead of SQLite.

| Aspect | Before | After |
|--------|--------|-------|
| Database | SQLite (File-based) | SQL Server |
| EF Provider | `UseSqlite()` | `UseSqlServer()` |
| Connection String | File path | Server connection |
| Database Location | Local file | SQL Server instance |
| Production Readiness | Development only | Enterprise-ready |

---

## ?? Files Modified

### 1. TaskManagement.API/Program.cs
```csharp
// ? UPDATED - Line 49-54
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);  // Changed from UseSqlite()
});
```

### 2. TaskManagement.API/appsettings.json
```json
// ? UPDATED - Connection strings section
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;",
    "DefaultConnectionWithCredentials": "Server=YOUR_SERVER;Database=TaskManagement;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
  }
}
```

---

## ?? Connection String Templates

### For Local Development (Windows Auth)
```
Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;
```
- Uses Windows user credentials
- Simple setup
- Recommended for development

### For Local Development (SQL Auth)
```
Server=localhost;Database=TaskManagement;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;
```
- Uses SQL Server login
- Requires SQL Server authentication enabled

### For SQL Server Express (Named Instance)
```
Server=localhost\SQLEXPRESS;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;
```
- For SQL Server Express installations
- Uses named instance

### For Remote Server
```
Server=192.168.1.100;Database=TaskManagement;User Id=appuser;Password=SecurePass123!;TrustServerCertificate=true;
```
- For production or remote SQL Server
- Include port if not default (1433)

### For Azure SQL Database
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TaskManagement;Persist Security Info=False;User ID=username@servername;Password=password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

---

## ?? Setup Commands

### Step 1: Verify SQL Server Status
```powershell
# Check if SQL Server service is running
Get-Service MSSQLSERVER | Select-Object Status

# Start SQL Server if stopped
Start-Service MSSQLSERVER

# Check SQL Server Express
Get-Service MSSQL$SQLEXPRESS | Select-Object Status
```

### Step 2: Apply Migrations
```bash
# Navigate to API project
cd TaskManagement.API

# Update database (creates database and tables)
dotnet ef database update

# View migration history
dotnet ef migrations list
```

### Step 3: Create Database User (SQL Server Auth)
```sql
-- SQL Query to execute in SQL Server Management Studio
CREATE LOGIN taskmanagement_user WITH PASSWORD = 'StrongPassword123!';
CREATE DATABASE TaskManagement;
USE TaskManagement;
CREATE USER taskmanagement_user FOR LOGIN taskmanagement_user;
ALTER ROLE db_owner ADD MEMBER taskmanagement_user;
```

### Step 4: Verify Installation
```sql
-- Check database exists
SELECT name FROM sys.databases WHERE name = 'TaskManagement';

-- View all tables
USE TaskManagement;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Count records in Tasks table
SELECT COUNT(*) as TaskCount FROM Tasks;
```

---

## ?? Configuration Examples

### Development Environment
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

### Production Environment
**File:** `appsettings.Production.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-prod-server;Database=TaskManagement;User Id=prod-user;Password=SecurePassword123!;TrustServerCertificate=true;Encrypt=true;Min Pool Size=10;Max Pool Size=100;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  }
}
```

---

## ?? Security Best Practices

### 1. Connection String in Production
? **DO NOT** hardcode passwords in appsettings.json

? **DO** use environment variables:
```csharp
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
```

### 2. Create Restricted SQL User
```sql
-- Create application user with minimal permissions
CREATE LOGIN app_user WITH PASSWORD = 'ComplexPassword123!@#';
USE TaskManagement;
CREATE USER app_user FOR LOGIN app_user;

-- Grant only necessary permissions (not db_owner)
GRANT SELECT, INSERT, UPDATE, DELETE ON Tasks TO app_user;
```

### 3. Enable Encryption (Production)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=TaskManagement;...;Encrypt=true;TrustServerCertificate=false;"
  }
}
```

---

## ?? Starting the Application

### From Visual Studio
1. Set `TaskManagement.API` as startup project
2. Press `F5` or click **Start Debugging**
3. Application opens at `https://localhost:5227`

### From Command Line
```bash
cd TaskManagement.API
dotnet run
```

### View Swagger UI
```
https://localhost:5227/swagger
```

---

## ?? Testing the Connection

### Using cURL
```bash
# Get all tasks
curl -X GET "https://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: tenant-001"

# Create a task
curl -X POST "https://localhost:5227/api/tasks" \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: tenant-001" \
  -d '{
    "title": "Test Task",
    "description": "Testing SQL Server connection",
    "priority": 1,
    "assignedTo": "developer@example.com"
  }'
```

### Using PowerShell
```powershell
$headers = @{
    'X-Tenant-Id' = 'tenant-001'
}

Invoke-WebRequest -Uri 'https://localhost:5227/api/tasks' `
  -Headers $headers `
  -Method GET
```

### Using Postman
1. Set request URL: `https://localhost:5227/api/tasks`
2. Go to **Headers** tab
3. Add: `X-Tenant-Id: tenant-001`
4. Click **Send**

---

## ?? Performance Tuning

### Connection Pooling Settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagement;Trusted_Connection=true;TrustServerCertificate=true;Min Pool Size=5;Max Pool Size=100;Connection Timeout=30;Command Timeout=60;"
  }
}
```

### EF Core Query Logging (Development Only)
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options
        .UseSqlServer(connectionString)
        .LogTo(Console.WriteLine)
        .EnableDetailedErrors();
});
```

---

## ?? Troubleshooting

### Error: "Cannot open database"
```
Solution: Run 'dotnet ef database update' to create the database
```

### Error: "Timeout expired"
```
Solution: Add connection timeout to connection string:
;Connection Timeout=60;
```

### Error: "Login failed for user"
```
Solution: Verify credentials in connection string:
- Check username spelling
- Verify password is correct
- Ensure user exists in SQL Server
```

### Error: "Named Pipes Provider could not open a connection"
```
Solution: Use correct instance name:
- Local: Server=localhost
- Express: Server=localhost\SQLEXPRESS
```

---

## ?? Quick Links

| Resource | Link |
|----------|------|
| **API Documentation** | [README_API_DOCUMENTATION.md](./README_API_DOCUMENTATION.md) |
| **SQL Server Setup Guide** | [SQL_SERVER_SETUP_GUIDE.md](./SQL_SERVER_SETUP_GUIDE.md) |
| **Quick Reference** | [SQL_SERVER_QUICK_REFERENCE.md](./SQL_SERVER_QUICK_REFERENCE.md) |
| **Migration Summary** | [SQLSERVER_MIGRATION_SUMMARY.md](./SQLSERVER_MIGRATION_SUMMARY.md) |
| **Main README** | [README.md](./README.md) |

---

## ? Verification Checklist

Run through these checks to verify everything is working:

```powershell
# 1. Check SQL Server is running
Get-Service MSSQLSERVER | Select-Object Status

# 2. Build the solution
dotnet build

# 3. Apply migrations
cd TaskManagement.API
dotnet ef database update

# 4. Run the application
dotnet run

# 5. Test an endpoint
curl -X GET "https://localhost:5227/api/tasks" -H "X-Tenant-Id: tenant-001"

# 6. Verify database in SSMS
# - Open SQL Server Management Studio
# - Connect to localhost
# - Navigate to Databases > TaskManagement
# - Expand Tables to see Tasks table
```

---

## ?? You're All Set!

Your Task Management API is now configured with SQL Server. 

**Next Steps:**
1. ? Verify SQL Server is running
2. ? Run migrations: `dotnet ef database update`
3. ? Start the app: `dotnet run`
4. ? Test endpoints: `https://localhost:5227/swagger`
5. ? Explore Swagger UI to test API

---

**Built with ?? using .NET 8 and SQL Server**

