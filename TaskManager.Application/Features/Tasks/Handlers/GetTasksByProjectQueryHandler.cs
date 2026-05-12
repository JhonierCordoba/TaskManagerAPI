using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Tasks.Queries;

namespace TaskManager.Application.Features.Tasks.Handlers;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, IEnumerable<TaskDto>>
{
    private readonly string _connectionString;

    public GetTasksByProjectQueryHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<IEnumerable<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = """
            SELECT Id, Title, Description, Status, Priority, DueDate, CreatedAt
            FROM Tasks
            WHERE ProjectId = @ProjectId
            """;

        var parameters = new DynamicParameters();
        parameters.Add("ProjectId", request.ProjectId);

        if (request.Status.HasValue)
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            sql += " AND Priority = @Priority";
            parameters.Add("Priority", request.Priority.Value);
        }

        sql += " ORDER BY CreatedAt DESC";

        return await connection.QueryAsync<TaskDto>(sql, parameters);
    }
}