using TaskManagement.Core.Interfaces;

namespace TaskManagement.Application.Features.Tasks.GetAll;

public class GetAllTasksHandler
{
    private readonly ITaskRepository _repository;

    public GetAllTasksHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetAllTasksResponse>> HandleAsync(GetAllTasksQuery query, string tenantId, CancellationToken cancellationToken = default)
    {
        var tasks = await _repository.GetAllAsync(tenantId, cancellationToken);

        var filteredTasks = tasks
            .Where(t => !t.IsDeleted)
            .Where(t => query.PriorityFilter == null || (int)t.Priority == query.PriorityFilter)
            .Where(t => query.StatusFilter == null || (int)t.Status == query.StatusFilter)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new GetAllTasksResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                AssignedTo = t.AssignedTo,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToList();

        return filteredTasks;
    }
}   