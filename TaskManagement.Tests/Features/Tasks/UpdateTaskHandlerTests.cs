using Xunit;
using Moq;
using TaskManagement.Application.Features.Tasks.Update;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;

namespace TaskManagement.Tests.Features.Tasks;

public class UpdateTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly UpdateTaskHandler _handler;

    public UpdateTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new UpdateTaskHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidUpdate_UpdatesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Original Title",
            Status = TaskStatus.Pending,
            Priority = TaskPriority.Low,
            TenantId = "tenant-1"
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Status = (int)TaskStatus.InProgress,
            Priority = (int)TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, "tenant-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem t, CancellationToken ct) => t);
        _repositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, "tenant-1");

        // Assert
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("InProgress", result.Status);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CompletedTask_ThrowsBusinessException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Completed Task",
            Status = TaskStatus.Completed,
            TenantId = "tenant-1"
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Status = (int)TaskStatus.InProgress,
            Priority = 0
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, "tenant-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.HandleAsync(command, "tenant-1"));

        Assert.Contains("Completed tasks cannot be modified.", exception.Errors);
    }

    [Fact]
    public async Task HandleAsync_HighPriorityWithoutDueDate_ThrowsBusinessException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Task",
            Status = TaskStatus.Pending,
            TenantId = "tenant-1"
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Status = (int)TaskStatus.Pending,
            Priority = (int)TaskPriority.High,
            DueDate = null
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, "tenant-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.HandleAsync(command, "tenant-1"));

        Assert.Contains("High priority tasks must have a DueDate.", exception.Errors);
    }
}