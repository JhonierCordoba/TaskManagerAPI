namespace TaskManager.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}