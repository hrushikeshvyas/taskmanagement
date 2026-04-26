using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;
using CoreTaskStatus = TaskManagement.Core.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Update;

public class UpdateTaskHandler
{
    private readonly ITaskRepository _repository;

    public UpdateTaskHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateTaskResponse> HandleAsync(UpdateTaskCommand command, string tenantId, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(command.Id, tenantId, cancellationToken);

        if (task == null || task.IsDeleted)
            throw new BusinessException("Task not found.", "TASK_NOT_FOUND");

        ValidateUpdate(task, command);

        task.Title = command.Title;
        task.Description = command.Description;
        task.Status = (CoreTaskStatus)command.Status;
        task.Priority = (TaskPriority)command.Priority;
        task.DueDate = command.DueDate;
        task.AssignedTo = command.AssignedTo;
        task.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(task, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new UpdateTaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            UpdatedAt = task.UpdatedAt
        };
    }

    private static void ValidateUpdate(Core.Entities.TaskItem task, UpdateTaskCommand command)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(command.Title))
            errors.Add("Title is required.");

        if (task.Status == CoreTaskStatus.Completed)
            errors.Add("Completed tasks cannot be modified.");

        if (task.Status == CoreTaskStatus.Cancelled)
            errors.Add("Cancelled tasks are read-only.");

        var newStatus = (CoreTaskStatus)command.Status;
        if (task.Status == CoreTaskStatus.Completed && newStatus == CoreTaskStatus.InProgress)
            errors.Add("Status cannot move from Completed to InProgress.");

        if ((TaskPriority)command.Priority == TaskPriority.High && !command.DueDate.HasValue)
            errors.Add("High priority tasks must have a DueDate.");

        if (command.DueDate.HasValue && command.DueDate < DateTime.UtcNow.Date)
            errors.Add("DueDate cannot be in the past.");

        if (errors.Count > 0)
            throw new BusinessException("Validation failed.", "VALIDATION_ERROR", errors);
    }
}