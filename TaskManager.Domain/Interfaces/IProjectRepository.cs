using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
}