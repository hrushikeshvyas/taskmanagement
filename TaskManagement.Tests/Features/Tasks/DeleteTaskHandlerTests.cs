using Xunit;
using Moq;
using TaskManagement.Application.Features.Tasks.Delete;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;

namespace TaskManagement.Tests.Features.Tasks;

public class DeleteTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly DeleteTaskHandler _handler;

    public DeleteTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new DeleteTaskHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidTask_SoftDeletesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Status = TaskStatus.Pending,
            TenantId = "tenant-1",
            IsDeleted = false
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, "tenant-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem t, CancellationToken ct) => t);
        _repositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(new DeleteTaskCommand { Id = taskId }, "tenant-1");

        // Assert
        Assert.True(task.IsDeleted);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, "tenant-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.HandleAsync(new DeleteTaskCommand { Id = taskId }, "tenant-1"));

        Assert.Equal("CANNOT_DELETE_COMPLETED_TASK", exception.Code);
        Assert.Equal("Completed tasks cannot be deleted.", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_TaskNotFound_ThrowsBusinessException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, "tenant-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.HandleAsync(new DeleteTaskCommand { Id = taskId }, "tenant-1"));

        Assert.Equal("TASK_NOT_FOUND", exception.Code);
    }
}