using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Projects.Queries;

namespace TaskManager.Application.Features.Projects.Handlers;

public class GetProjectsByUserQueryHandler : IRequestHandler<GetProjectsByUserQuery, IEnumerable<ProjectDto>>
{
    private readonly string _connectionString;

    public GetProjectsByUserQueryHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsByUserQuery request, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);

        const string sql = """
            SELECT Id, Name, Description, CreatedAt
            FROM Projects
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryAsync<ProjectDto>(sql, new { request.UserId });
    }
}