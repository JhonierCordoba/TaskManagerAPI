using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Enums.TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid ProjectId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Project Project { get; private set; }

    private TaskItem() { }

    public static TaskItem Create(string title, string? description, TaskPriority priority, DateTime? dueDate, Guid projectId)
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Status = Enums.TaskStatus.Pending,
            Priority = priority,
            DueDate = dueDate,
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string? description, TaskPriority priority, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
    }

    public void ChangeStatus(Enums.TaskStatus status)
    {
        Status = status;
    }
}