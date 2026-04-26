namespace TaskManagement.Application.Features.Tasks.GetAll;

public class GetAllTasksQuery
{
    public int? PriorityFilter { get; set; }
    public int? StatusFilter { get; set; }
}