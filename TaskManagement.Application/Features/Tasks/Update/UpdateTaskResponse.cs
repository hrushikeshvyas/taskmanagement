namespace TaskManagement.Application.Features.Tasks.Update;

public class UpdateTaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
}