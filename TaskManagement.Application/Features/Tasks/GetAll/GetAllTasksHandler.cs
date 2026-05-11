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
        //query.PriorityFilter = query.PriorityFilter ?? 0;
        //query.StatusFilter = query.StatusFilter ?? 0;
        if (query.PriorityFilter == null && query.StatusFilter == null)
        {
            var filteredTasks = tasks
                .Where(t => !t.IsDeleted)
                //.Where(t => (int)t.Priority == query.PriorityFilter || (int)t.Status == query.StatusFilter)
                //.Where(t => query.StatusFilter == null || (int)t.Status == query.StatusFilter)
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
        else
        {
            var filteredTasks = tasks
                .Where(t => !t.IsDeleted)
                .Where(t => (int)t.Priority == query.PriorityFilter || (int)t.Status == query.StatusFilter)
                //.Where(t => query.StatusFilter == null || (int)t.Status == query.StatusFilter)
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
}   