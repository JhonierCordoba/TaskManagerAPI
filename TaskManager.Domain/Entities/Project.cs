namespace TaskManager.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; }
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    private Project() { }

    public static Project Create(string name, string? description, Guid userId)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}