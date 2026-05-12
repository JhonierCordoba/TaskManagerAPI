using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Projects.Queries;

namespace TaskManager.Application.Features.Projects.Handlers;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly string _connectionString;

    public GetProjectByIdQueryHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);

        const string sql = """
            SELECT Id, Name, Description, CreatedAt
            FROM Projects
            WHERE Id = @Id AND UserId = @UserId
            """;

        var project = await connection.QueryFirstOrDefaultAsync<ProjectDto>(sql, new { request.Id, request.UserId });

        return project ?? throw new NotFoundException("Project", request.Id);
    }
}