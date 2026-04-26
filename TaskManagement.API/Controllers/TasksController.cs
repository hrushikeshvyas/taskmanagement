using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Add this using directive to fix CS0246 for [ProducesResponseType]
using TaskManagement.Application.Features.Tasks.Create;
using TaskManagement.Application.Features.Tasks.Delete;
using TaskManagement.Application.Features.Tasks.GetAll;
using TaskManagement.Application.Features.Tasks.GetById;
using TaskManagement.Application.Features.Tasks.Update;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly CreateTaskHandler _createHandler;
    private readonly GetAllTasksHandler _getAllHandler;
    private readonly GetByIdHandler _getByIdHandler;
    private readonly UpdateTaskHandler _updateHandler;
    private readonly DeleteTaskHandler _deleteHandler;

    public TasksController(
        CreateTaskHandler createHandler,
        GetAllTasksHandler getAllHandler,
        GetByIdHandler getByIdHandler,
        UpdateTaskHandler updateHandler,
        DeleteTaskHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    private string GetTenantId() => HttpContext.Items[TenantConstants.TenantIdHeader]?.ToString() ?? "";

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateTaskResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CreateTaskResponse>>> CreateTask(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(command, GetTenantId(), cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { id = result.Id },
            ApiResponse<CreateTaskResponse>.SuccessResponse(result, "Task created successfully."));
    }

    /// <summary>
    /// Get all tasks with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<GetAllTasksResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetAllTasksResponse>>>> GetAllTasks(
        [FromQuery] int? priorityFilter,
        [FromQuery] int? statusFilter,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTasksQuery { PriorityFilter = priorityFilter, StatusFilter = statusFilter };
        var result = await _getAllHandler.HandleAsync(query, GetTenantId(), cancellationToken);
        return Ok(ApiResponse<List<GetAllTasksResponse>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get a task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetByIdResponse>>> GetTask(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdQuery { Id = id };
        var result = await _getByIdHandler.HandleAsync(query, GetTenantId(), cancellationToken);
        return Ok(ApiResponse<GetByIdResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UpdateTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UpdateTaskResponse>>> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _updateHandler.HandleAsync(command, GetTenantId(), cancellationToken);
        return Ok(ApiResponse<UpdateTaskResponse>.SuccessResponse(result, "Task updated successfully."));
    }

    /// <summary>
    /// Delete a task (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand { Id = id };
        await _deleteHandler.HandleAsync(command, GetTenantId(), cancellationToken);
        return NoContent();
    }
}