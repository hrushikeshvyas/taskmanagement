# Task Management API - Production Ready

A professional **REST API** for managing tasks in a **multi-tenant environment** built with **.NET 8**, **ASP.NET Core**, and **Entity Framework Core**.

---

## 📖 API Documentation

> **👉 For detailed API documentation with comprehensive examples, curl commands, and request/response samples, please visit:**
> 
> ### **[📚 API DOCUMENTATION GUIDE](./README_API_DOCUMENTATION.md)**
>
> This guide includes:
> - ✅ Complete endpoint documentation
> - ✅ X-Tenant-Id header specifications
> - ✅ Request/Response examples for all endpoints
> - ✅ cURL command examples
> - ✅ Real-world usage scenarios
> - ✅ Multi-tenancy architecture explanation
> - ✅ Troubleshooting guide

---

## 🏗️ Architecture

This solution implements **Vertical Slice Architecture**, organizing features vertically rather than by technical layers.

Each feature is **self-contained** and **independently testable**.

## 📦 Project Structure

## ✨ Key Features

### ✅ Multi-Tenancy
- **Required** `X-Tenant-Id` header on all requests
- Automatic tenant validation via middleware
- Complete tenant isolation at database level

### ✅ Business Rules
- ❌ Completed tasks **cannot be deleted**
- ❌ Status **cannot move** from Completed → InProgress
- ⚠️ High priority tasks **must have** a DueDate
- 🔒 Cancelled tasks are **read-only**

### ✅ Soft Delete
- Tasks marked as deleted, never physically removed
- Maintains data integrity and audit trail
- Automatic exclusion from queries

### ✅ Production-Ready
- Comprehensive input validation
- Exception handling middleware
- Swagger/OpenAPI documentation
- Structured error responses
- Fully testable design

## 🚀 Quick Start

### Prerequisites
- **.NET 8 SDK** or latest
- **Visual Studio 2022** (optional)
- **SQLite** (included with .NET)

### 1. Clone & Navigate
git clone <repository-url> cd TaskManagement.Solution
### 2. Restore & Build
dotnet restore dotnet build
### 3. Run Migrations
cd TaskManagement.API dotnet ef database update
### 4. Start the Application
dotnet run

API available at: **https://localhost:7001**

Swagger UI: **https://localhost:7001/swagger/ui**

---

## 🔌 API Endpoints

All endpoints require `X-Tenant-Id` header.

### Create Task
- **POST** `/api/tasks`
- POST /api/tasks HTTP/1.1 X-Tenant-Id: acme-corp Content-Type: application/json
{ "title": "Implement authentication", "description": "Add JWT token support", "priority": 2, "dueDate": "2024-12-31", "assignedTo": "john@example.com" }

**Response (201 Created):**
````````
{ "success": true, "message": "Task created successfully.", "data": { "id": "550e8400-e29b-41d4-a716-446655440000", "title": "Implement authentication", "status": "Pending", "priority": "High", "dueDate": "2024-12-31", "createdAt": "2024-01-15T10:30:00Z" } }

# Response
````````

### Get All Tasks
- **GET** `/api/tasks`
- GET /api/tasks HTTP/1.1 X-Tenant-Id: acme-corp

**Response (200 OK):**
````````
{ "success": true, "data": [ { "id": "550e8400-e29b-41d4-a716-446655440000", "title": "Implement authentication", "status": "Pending", "priority": "High", "dueDate": "2024-12-31", "createdAt": "2024-01-15T10:30:00Z" }, { "id": "550e8400-e29b-41d4-a716-446655440001", "title": "Design database schema", "status": "Completed", "priority": "Medium", "dueDate": "2024-01-31", "createdAt": "2024-01-10T14:20:00Z" } ] }

# Response
````````

### Get Task By ID
- **GET** `/api/tasks/{id}`
- GET /api/tasks/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1 X-Tenant-Id: acme-corp

**Response (200 OK):**
````````
{ "success": true, "data": { "id": "550e8400-e29b-41d4-a716-446655440000", "title": "Implement authentication", "description": "Add JWT token support", "status": "Pending", "priority": "High", "dueDate": "2024-12-31", "createdAt": "2024-01-15T10:30:00Z" } }

# Response
````````

### Update Task
- **PUT** `/api/tasks/{id}`
- PUT /api/tasks/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1 X-Tenant-Id: acme-corp Content-Type: application/json
{ "title": "Implement authentication - updated", "description": "Add JWT token support with refresh tokens", "priority": 1, "dueDate": "2024-11-30", "assignedTo": "jane@example.com" }

**Response (200 OK):**
````````
{ "success": true, "message": "Task updated successfully.", "data": { "id": "550e8400-e29b-41d4-a716-446655440000", "title": "Implement authentication - updated", "status": "Pending", "priority": "Critical", "dueDate": "2024-11-30", "createdAt": "2024-01-15T10:30:00Z", "updatedAt": "2024-02-01T12:45:00Z" } }

# Response
````````

### Delete Task (Soft Delete)
- **DELETE** `/api/tasks/{id}`
- DELETE /api/tasks/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1 X-Tenant-Id: acme-corp

**Response (204 No Content):**
````````
{ "success": true, "message": "Task deleted successfully." }

# Response
````````

## 📊 Enumerations
0 = Pending 1 = InProgress 2 = Completed 3 = Cancelled
### TaskStatus

````````markdown
### TaskPriority
````````
0 = Low 1 = Medium 2 = High

# Response

````````

---

## 🧪 Testing

### Run All Tests
dotena/test

### Run Specific Test Class
````````
dotnet test --filter ClassName=CreateTaskHandlerTests
# Response

````````

### Test Coverage
- ✅ CreateTaskHandler
- ✅ DeleteTaskHandler
- ✅ UpdateTaskHandler

---

## ⚙️ Configuration

### appsettings.json
{ "ConnectionStrings": { "DefaultConnection": "Data Source=taskmanagement.db" } }
### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`

### Switch to SQL Server
**Update appsettings.json:**
````````
{ "ConnectionStrings": { "DefaultConnection": "Server=tcp:your_sql_server,1433;Database=taskmanagement;User Id=your_username;Password=your_password;" } }
{ "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=TaskManagement;Trusted_Connection=true;" } }

# Response

````````

**Update Program.cs:**
````````
options.UseSqlServer(connectionString);

# Response

````````

---

## 📋 HTTP Status Codes

| Code | Meaning |
|------|---------|
| `200` | ✅ Success |
| `201` | ✅ Created |
| `204` | ✅ No Content (Delete) |
| `400` | ❌ Bad Request / Business Rule Violation |
| `404` | ❌ Not Found |
| `500` | ❌ Internal Server Error |

---

## 📝 Error Response Format

````````
{
  "success": false,
  "error": {
    "code": "Error_Code",
    "message": "Detailed error message.",
    "validationErrors": [
      { "field": "dueDate", "message": "Due date is required for high priority tasks." }
    ]
  }
}
{ "success": false, "message": "Validation failed.", "errors": [ "High priority tasks must have a DueDate.", "Title is required." ] }
# Response

````````

---

## 🏭 Production Deployment

### Recommended Changes:
1. **Database**: Switch to SQL Server
2. **Logging**: Integrate Serilog for structured logging
3. **Authentication**: Add JWT/OAuth2
4. **Rate Limiting**: Implement per-tenant limits
5. **Monitoring**: Add Application Insights
6. **CORS**: Configure allowed origins
7. **API Versioning**: Implement header-based versioning

### Build for Production
dotnet publish -c Release -o ./publish

---

## 📚 Architecture Decisions

| Decision | Why? |
|----------|------|
| **Vertical Slice** | Self-contained features, easier to modify/test |
| **Handler Pattern** | Simple alternative to CQRS without complexity |
| **Soft Delete** | Data retention, audit trail, recovery support |
| **Tenant Middleware** | Early validation, enforces multi-tenancy |
| **Repository Pattern** | Decouples data access, enables testing |
| **Domain Enums** | Type-safe status/priority values |

---

## 🤝 Contributing

See `CONTRIBUTING.md` for guidelines.

---

## 📄 License

MIT License - See LICENSE file

---

## 💡 Future Enhancements

- [ ] Event Sourcing for audit trail
- [ ] Background jobs (Hangfire)
- [ ] Advanced filtering & pagination
- [ ] Batch operations
- [ ] WebSocket support for real-time updates
- [ ] GraphQL endpoint
- [ ] Rate limiting
- [ ] Caching layer (Redis)

---

## 🆘 Support

For issues, open a GitHub issue or contact the development team.

---

**Built with ❤️ using .NET 8 & ASP.NET Core**
