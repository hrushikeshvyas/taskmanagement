namespace TaskManagement.Application.Features.Tasks.Create;

public class CreateTaskCommand
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? AssignedTo { get; set; }
}