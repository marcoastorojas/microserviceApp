using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using Shared.Domain.Events;

namespace Shared.Infrastructure.Events;

public class NpgsqlDomainEventFailRespository(IDbConnection connection) : DomainEventFailRepository
{
    private readonly IDbConnection _dbConnection = connection;

    public Task<List<DomainEvent>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task Save(DomainEvent domainEvent)
    {
        var parameters = new
        {
            Id = domainEvent.EventId,
            domainEvent.AggregateId,
            Body = JsonSerializer.Serialize(domainEvent.ToPrimitives()),
            EventName = domainEvent.EventName(),
            OcurredOn = domainEvent.OccurredOn
        };

        var sql = @"
            insert into domainEventsFailed 
            (id,aggregateId,body,eventName,ocurredOn)
            values (@Id,@AggregateId,@Body::jsonb,@EventName,@OcurredOn)
        ";
        await _dbConnection.QueryAsync(sql, parameters);
    }
}