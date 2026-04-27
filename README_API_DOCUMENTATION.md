# Task Management API Documentation

## Overview

The Task Management API is a multi-tenant RESTful API built with .NET 8 using Vertical Slice Architecture. It provides endpoints for managing tasks with support for multiple tenants through the `X-Tenant-Id` header.

## Base URL

```
http://localhost:5227/api
```

## Authentication & Multi-Tenancy

### X-Tenant-Id Header

All API requests (except Swagger documentation endpoints) **require** the `X-Tenant-Id` header. This header identifies which tenant the request belongs to.

**Header Specifications:**
- **Header Name:** `X-Tenant-Id`
- **Type:** String
- **Required:** Yes (for all endpoints except Swagger)
- **Min Length:** 1 character
- **Max Length:** 50 characters
- **Location:** Request Header

**Example Header:**
```
X-Tenant-Id: tenant-001
```

### How to Pass X-Tenant-Id Header

#### cURL Example
```bash
curl -X GET "http://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: tenant-001"
```

#### JavaScript/Fetch
```javascript
const headers = {
  'X-Tenant-Id': 'tenant-001'
};

fetch('http://localhost:5227/api/tasks', {
  method: 'GET',
  headers: headers
});
```

#### Postman
1. Go to the "Headers" tab
2. Add a new header with Key: `X-Tenant-Id` and Value: `tenant-001`

#### C# HttpClient
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Tenant-Id", "tenant-001");
var response = await client.GetAsync("http://localhost:5227/api/tasks");
```

---

## API Response Format

All API responses follow a consistent structure:

```json
{
  "success": true,
  "data": {},
  "message": "Success message",
  "errors": []
}
```

**Fields:**
- `success` (boolean): Indicates if the request was successful
- `data` (object): The response data (varies by endpoint)
- `message` (string): A message describing the result
- `errors` (array): Array of error messages (if any)

---

## Error Responses

### Missing X-Tenant-Id Header

**Status Code:** 400 Bad Request

```json
{
  "success": false,
  "data": null,
  "message": "Missing required header: X-Tenant-Id",
  "errors": []
}
```

### Invalid X-Tenant-Id Header

**Status Code:** 400 Bad Request

```json
{
  "success": false,
  "data": null,
  "message": "Invalid X-Tenant-Id header value.",
  "errors": []
}
```

### Not Found

**Status Code:** 404 Not Found

```json
{
  "success": false,
  "data": null,
  "message": "Task not found",
  "errors": []
}
```

---

## Enum Reference

### Task Priority

| Value | Code |
|-------|------|
| Low | 0 |
| Medium | 1 |
| High | 2 |

### Task Status

| Value | Code |
|-------|------|
| Pending | 0 |
| InProgress | 1 |
| Completed | 2 |
| Cancelled | 3 |

---

## Endpoints

### 1. Create Task

Creates a new task for the specified tenant.

**Endpoint:** `POST /api/tasks`

**Headers:**
```
X-Tenant-Id: tenant-001
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Complete project documentation",
  "description": "Write comprehensive API documentation for the task management system",
  "priority": 1,
  "dueDate": "2024-12-31T23:59:59Z",
  "assignedTo": "john.doe@example.com"
}
```

**Request Body Schema:**
- `title` (string, required): Task title (max 255 characters)
- `description` (string, optional): Detailed task description
- `priority` (integer, required): Task priority (0=Low, 1=Medium, 2=High)
- `dueDate` (datetime, optional): Task due date in ISO 8601 format
- `assignedTo` (string, optional): Email or identifier of the person assigned to the task

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Complete project documentation",
    "status": "Pending",
    "priority": "Medium",
    "dueDate": "2024-12-31T23:59:59Z",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "message": "Task created successfully.",
  "errors": []
}
```

**Response Fields:**
- `id` (guid): Unique identifier for the created task
- `title` (string): Task title
- `status` (string): Initial status (always "Pending" for new tasks)
- `priority` (string): Task priority name
- `dueDate` (datetime): Task due date
- `createdAt` (datetime): Timestamp when task was created

**cURL Example:**
```bash
curl -X POST "http://localhost:5227/api/tasks" \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: tenant-001" \
  -d '{
    "title": "Complete project documentation",
    "description": "Write comprehensive API documentation for the task management system",
    "priority": 1,
    "dueDate": "2024-12-31T23:59:59Z",
    "assignedTo": "john.doe@example.com"
  }'
```

**Possible Status Codes:**
- `201 Created`: Task created successfully
- `400 Bad Request`: Invalid request body or missing X-Tenant-Id header
- `500 Internal Server Error`: Server error

---

### 2. Get All Tasks

Retrieves all tasks for the specified tenant with optional filtering.

**Endpoint:** `GET /api/tasks`

**Headers:**
```
X-Tenant-Id: tenant-001
```

**Query Parameters:**
- `priorityFilter` (integer, optional): Filter by priority (0=Low, 1=Medium, 2=High)
- `statusFilter` (integer, optional): Filter by status (0=Pending, 1=InProgress, 2=Completed, 3=Cancelled)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Complete project documentation",
      "description": "Write comprehensive API documentation for the task management system",
      "status": "Pending",
      "priority": "Medium",
      "dueDate": "2024-12-31T23:59:59Z",
      "assignedTo": "john.doe@example.com",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "title": "Review code changes",
      "description": "Review pull requests for the authentication module",
      "status": "InProgress",
      "priority": "High",
      "dueDate": "2024-01-20T23:59:59Z",
      "assignedTo": "jane.smith@example.com",
      "createdAt": "2024-01-14T14:20:00Z",
      "updatedAt": "2024-01-15T09:15:00Z"
    }
  ],
  "message": "Success",
  "errors": []
}
```

**Response Fields:**
- `id` (guid): Task unique identifier
- `title` (string): Task title
- `description` (string): Task description
- `status` (string): Current task status
- `priority` (string): Task priority name
- `dueDate` (datetime): Task due date
- `assignedTo` (string): Person assigned to the task
- `createdAt` (datetime): Timestamp when task was created
- `updatedAt` (datetime): Timestamp when task was last updated

**cURL Example - Get All Tasks:**
```bash
curl -X GET "http://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: tenant-001"
```

**cURL Example - Get High Priority Tasks:**
```bash
curl -X GET "http://localhost:5227/api/tasks?priorityFilter=2" \
  -H "X-Tenant-Id: tenant-001"
```

**cURL Example - Get Completed Tasks:**
```bash
curl -X GET "http://localhost:5227/api/tasks?statusFilter=2" \
  -H "X-Tenant-Id: tenant-001"
```

**cURL Example - Get High Priority + InProgress Tasks:**
```bash
curl -X GET "http://localhost:5227/api/tasks?priorityFilter=2&statusFilter=1" \
  -H "X-Tenant-Id: tenant-001"
```

**Possible Status Codes:**
- `200 OK`: Successfully retrieved tasks
- `400 Bad Request`: Missing X-Tenant-Id header

---

### 3. Get Task by ID

Retrieves a specific task by its ID.

**Endpoint:** `GET /api/tasks/{id}`

**Parameters:**
- `id` (guid, path parameter): The unique identifier of the task

**Headers:**
```
X-Tenant-Id: tenant-001
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Complete project documentation",
    "description": "Write comprehensive API documentation for the task management system",
    "status": "Pending",
    "priority": "Medium",
    "dueDate": "2024-12-31T23:59:59Z",
    "assignedTo": "john.doe@example.com",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  },
  "message": "Success",
  "errors": []
}
```

**Response Fields:**
- `id` (guid): Task unique identifier
- `title` (string): Task title
- `description` (string): Task description
- `status` (string): Current task status
- `priority` (string): Task priority name
- `dueDate` (datetime): Task due date
- `assignedTo` (string): Person assigned to the task
- `createdAt` (datetime): Timestamp when task was created
- `updatedAt` (datetime): Timestamp when task was last updated

**cURL Example:**
```bash
curl -X GET "http://localhost:5227/api/tasks/550e8400-e29b-41d4-a716-446655440000" \
  -H "X-Tenant-Id: tenant-001"
```

**Possible Status Codes:**
- `200 OK`: Task retrieved successfully
- `400 Bad Request`: Missing X-Tenant-Id header
- `404 Not Found`: Task not found

---

### 4. Update Task

Updates an existing task.

**Endpoint:** `PUT /api/tasks/{id}`

**Parameters:**
- `id` (guid, path parameter): The unique identifier of the task to update

**Headers:**
```
X-Tenant-Id: tenant-001
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Complete project documentation - UPDATED",
  "description": "Write comprehensive API documentation for the task management system with examples",
  "status": 1,
  "priority": 2,
  "dueDate": "2024-12-25T23:59:59Z",
  "assignedTo": "jane.smith@example.com"
}
```

**Request Body Schema:**
- `title` (string, required): Updated task title
- `description` (string, optional): Updated task description
- `status` (integer, required): Updated status (0=Pending, 1=InProgress, 2=Completed, 3=Cancelled)
- `priority` (integer, required): Updated priority (0=Low, 1=Medium, 2=High)
- `dueDate` (datetime, optional): Updated due date
- `assignedTo` (string, optional): Updated assignee

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Complete project documentation - UPDATED",
    "status": "InProgress",
    "priority": "High",
    "updatedAt": "2024-01-15T15:45:00Z"
  },
  "message": "Task updated successfully.",
  "errors": []
}
```

**Response Fields:**
- `id` (guid): Task unique identifier
- `title` (string): Updated task title
- `status` (string): Updated task status
- `priority` (string): Updated task priority
- `updatedAt` (datetime): Timestamp when task was updated

**cURL Example:**
```bash
curl -X PUT "http://localhost:5227/api/tasks/550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: tenant-001" \
  -d '{
    "title": "Complete project documentation - UPDATED",
    "description": "Write comprehensive API documentation for the task management system with examples",
    "status": 1,
    "priority": 2,
    "dueDate": "2024-12-25T23:59:59Z",
    "assignedTo": "jane.smith@example.com"
  }'
```

**Possible Status Codes:**
- `200 OK`: Task updated successfully
- `400 Bad Request`: Invalid request body or missing X-Tenant-Id header
- `404 Not Found`: Task not found

---

### 5. Delete Task (Soft Delete)

Deletes a task (performs a soft delete).

**Endpoint:** `DELETE /api/tasks/{id}`

**Parameters:**
- `id` (guid, path parameter): The unique identifier of the task to delete

**Headers:**
```
X-Tenant-Id: tenant-001
```

**Response (204 No Content):**

A successful delete returns an empty response with status code 204.

**cURL Example:**
```bash
curl -X DELETE "http://localhost:5227/api/tasks/550e8400-e29b-41d4-a716-446655440000" \
  -H "X-Tenant-Id: tenant-001"
```

**Possible Status Codes:**
- `204 No Content`: Task deleted successfully
- `400 Bad Request`: Invalid request or missing X-Tenant-Id header
- `404 Not Found`: Task not found

---

## Usage Examples

### Scenario 1: Create and Retrieve a Task

**Step 1: Create a new task**
```bash
curl -X POST "http://localhost:5227/api/tasks" \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: tenant-001" \
  -d '{
    "title": "Implement user authentication",
    "description": "Add JWT-based authentication to the API",
    "priority": 2,
    "dueDate": "2024-02-01T00:00:00Z",
    "assignedTo": "developer@example.com"
  }'
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "12345678-1234-1234-1234-123456789012",
    "title": "Implement user authentication",
    "status": "Pending",
    "priority": "High",
    "dueDate": "2024-02-01T00:00:00Z",
    "createdAt": "2024-01-15T10:00:00Z"
  },
  "message": "Task created successfully.",
  "errors": []
}
```

**Step 2: Retrieve the created task**
```bash
curl -X GET "http://localhost:5227/api/tasks/12345678-1234-1234-1234-123456789012" \
  -H "X-Tenant-Id: tenant-001"
```

---

### Scenario 2: Filter Tasks by Priority and Status

**Get all high priority, in-progress tasks**
```bash
curl -X GET "http://localhost:5227/api/tasks?priorityFilter=2&statusFilter=1" \
  -H "X-Tenant-Id: tenant-001"
```

---

### Scenario 3: Update Task Status to Completed

**Mark a task as completed**
```bash
curl -X PUT "http://localhost:5227/api/tasks/12345678-1234-1234-1234-123456789012" \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: tenant-001" \
  -d '{
    "title": "Implement user authentication",
    "description": "Add JWT-based authentication to the API",
    "status": 2,
    "priority": 2,
    "dueDate": "2024-02-01T00:00:00Z",
    "assignedTo": "developer@example.com"
  }'
```

---

### Scenario 4: Delete a Task

**Delete a task**
```bash
curl -X DELETE "http://localhost:5227/api/tasks/12345678-1234-1234-1234-123456789012" \
  -H "X-Tenant-Id: tenant-001"
```

---

## Multi-Tenant Architecture

The API is designed to support multiple tenants. Each request is isolated to a specific tenant identified by the `X-Tenant-Id` header.

**Key Points:**
- All tasks are scoped to their tenant
- A tenant cannot access another tenant's tasks
- The `X-Tenant-Id` header validates that requests belong to the appropriate tenant
- All data filtering and isolation is automatically handled at the repository level

**Example with Different Tenants:**

```bash
# Tenant 1 retrieves their tasks
curl -X GET "http://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: tenant-001"

# Tenant 2 retrieves their tasks (separate data)
curl -X GET "http://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: tenant-002"
```

---

## Rate Limiting and Best Practices

1. **Always include X-Tenant-Id header** - This is mandatory for all endpoints except Swagger documentation
2. **Use appropriate HTTP methods** - GET for retrieval, POST for creation, PUT for updates, DELETE for deletion
3. **Handle error responses** - Check the `success` field in the response
4. **Use query parameters for filtering** - Reduce payload by filtering on the server side
5. **Use ISO 8601 format for dates** - Ensure consistent date/time formatting

---

## Troubleshooting

### Missing X-Tenant-Id Header

**Error:** 400 Bad Request - "Missing required header: X-Tenant-Id"

**Solution:** Add the `X-Tenant-Id` header to your request:
```bash
curl -X GET "http://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: tenant-001"
```

---

### Invalid X-Tenant-Id Header

**Error:** 400 Bad Request - "Invalid X-Tenant-Id header value."

**Possible Causes:**
- X-Tenant-Id is empty or null
- X-Tenant-Id exceeds 50 characters
- X-Tenant-Id is less than 1 character

**Solution:** Ensure the X-Tenant-Id header is between 1-50 characters:
```bash
curl -X GET "http://localhost:5227/api/tasks" \
  -H "X-Tenant-Id: valid-tenant-id"
```

---

### Task Not Found

**Error:** 404 Not Found - "Task not found"

**Possible Causes:**
- Task ID doesn't exist
- Task belongs to a different tenant
- Task was deleted

**Solution:** Verify the task ID and that it belongs to the correct tenant

---

## Swagger UI

You can also explore the API using Swagger UI:

```
http://localhost:5227/swagger/index.html
```

**Note:** The Swagger UI does not require the X-Tenant-Id header to load, but all actual API calls need it.

---

## Summary

| Method | Endpoint | Description | X-Tenant-Id Required |
|--------|----------|-------------|----------------------|
| POST | `/api/tasks` | Create a new task | Yes |
| GET | `/api/tasks` | Get all tasks (with optional filters) | Yes |
| GET | `/api/tasks/{id}` | Get a specific task | Yes |
| PUT | `/api/tasks/{id}` | Update a task | Yes |
| DELETE | `/api/tasks/{id}` | Delete a task | Yes |

---

## Support

For issues or questions about the API, please refer to the project documentation or contact the development team.

