using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;
using TaskStatus = TaskManagement.Core.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Create;

public class CreateTaskHandler
{
    private readonly ITaskRepository _repository;

    public CreateTaskHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTaskResponse> HandleAsync(CreateTaskCommand command, string tenantId, CancellationToken cancellationToken = default)
    {
        ValidateCommand(command);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Priority = (TaskPriority)command.Priority,
            DueDate = command.DueDate,
            AssignedTo = command.AssignedTo,
            TenantId = tenantId,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(task, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CreateTaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };
    }

    private static void ValidateCommand(CreateTaskCommand command)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(command.Title))
            errors.Add("Title is required.");

        if (command.Title?.Length > 200)
            errors.Add("Title cannot exceed 200 characters.");

        if (command.Priority == (int)TaskPriority.High && !command.DueDate.HasValue)
            errors.Add("High priority tasks must have a DueDate.");

        if (command.DueDate.HasValue && command.DueDate < DateTime.UtcNow.Date)
            errors.Add("DueDate cannot be in the past.");

        if (errors.Count > 0)
            throw new BusinessException("Validation failed.", "VALIDATION_ERROR", errors);
    }
}