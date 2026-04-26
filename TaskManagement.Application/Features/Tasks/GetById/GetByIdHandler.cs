using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;

namespace TaskManagement.Application.Features.Tasks.GetById;

public class GetByIdHandler
{
    private readonly ITaskRepository _repository;

    public GetByIdHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetByIdResponse> HandleAsync(GetByIdQuery query, string tenantId, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(query.Id, tenantId, cancellationToken);

        if (task == null || task.IsDeleted)
            throw new BusinessException("Task not found.", "TASK_NOT_FOUND");

        return new GetByIdResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            AssignedTo = task.AssignedTo,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}