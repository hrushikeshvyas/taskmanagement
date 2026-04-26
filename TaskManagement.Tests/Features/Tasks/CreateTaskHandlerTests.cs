using Xunit;
using Moq;
using TaskManagement.Application.Features.Tasks.Create;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Shared.Exceptions;

namespace TaskManagement.Tests.Features.Tasks;

public class CreateTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new CreateTaskHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = 1,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem task, CancellationToken ct) => task);
        _repositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, "tenant-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Pending", result.Status);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_HighPriorityWithoutDueDate_ThrowsBusinessException()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "High Priority Task",
            Priority = 2,
            DueDate = null
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.HandleAsync(command, "tenant-1"));

        Assert.Equal("VALIDATION_ERROR", exception.Code);
        Assert.Contains("High priority tasks must have a DueDate.", exception.Errors);
    }

    [Fact]
    public async Task HandleAsync_EmptyTitle_ThrowsBusinessException()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "",
            Priority = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.HandleAsync(command, "tenant-1"));

        Assert.Equal("VALIDATION_ERROR", exception.Code);
        Assert.Contains("Title is required.", exception.Errors);
    }
}