using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;
using CoreTaskStatus = TaskManagement.Core.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Delete;

public class DeleteTaskHandler
{
    private readonly ITaskRepository _repository;

    public DeleteTaskHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeleteTaskCommand command, string tenantId, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(command.Id, tenantId, cancellationToken);

        if (task == null || task.IsDeleted)
            throw new BusinessException("Task not found.", "TASK_NOT_FOUND");

        if (task.Status == CoreTaskStatus.Completed)
            throw new BusinessException("Completed tasks cannot be deleted.", "CANNOT_DELETE_COMPLETED_TASK");

        task.IsDeleted = true;
        task.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(task, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}