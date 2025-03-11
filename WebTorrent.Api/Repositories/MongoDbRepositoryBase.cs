using WebTorrent.Api.Helpers;
using WebTorrent.Api.Models;
using WebTorrent.Api.Repositories;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace WebTorrent.Api.Repositories;

public interface IRepository<TAggregateRoot, TId> where TAggregateRoot : Entity
{
    Task<IEnumerable<TAggregateRoot>> GetAllAsync(CancellationToken cancellationToken);

    Task<TAggregateRoot?> GetByIdAsync(TId id);

    IQueryable<TAggregateRoot> FindQueryable(Expression<Func<TAggregateRoot, bool>> expression, Func<IQueryable<TAggregateRoot>, IOrderedQueryable<TAggregateRoot>>? orderBy = null);

    Task<IEnumerable<TAggregateRoot>> WhereAsync(Expression<Func<TAggregateRoot, bool>>? expression, Func<IQueryable<TAggregateRoot>, IOrderedQueryable<TAggregateRoot>>? orderBy = null, CancellationToken cancellationToken = default);

    Task<TAggregateRoot?> SingleOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> expression);

    Task<TAggregateRoot> AddAsync(TAggregateRoot entity);

    Task UpdateAsync(TAggregateRoot entity);

    Task UpdateRangeAsync(IEnumerable<TAggregateRoot> entities);

    Task DeleteAsync(TAggregateRoot entity);
}

public class MongoDbRepositoryBase<TAggregateRoot, TId> : IRepository<TAggregateRoot, TId> where TAggregateRoot : Entity
{
    private readonly string _username;
    protected IMongoCollection<TAggregateRoot> _collection;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IClaimReader _claimReader;

    public MongoDbRepositoryBase(IMongoDatabase database,
    IDateTimeProvider dateTimeProvider, IClaimReader claimReader)
    {
        _collection = database.GetCollection<TAggregateRoot>(typeof(TAggregateRoot).Name);
        _dateTimeProvider = dateTimeProvider;
        _username = claimReader.GetUsernameFromClaim();
        _claimReader = claimReader;
    }

    public async Task<IEnumerable<TAggregateRoot>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<TAggregateRoot>.Filter.Empty;
        var result = await _collection.Find(filter).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<TAggregateRoot?> GetByIdAsync(TId id)
    {
        var filter = Builders<TAggregateRoot>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public IQueryable<TAggregateRoot> FindQueryable(Expression<Func<TAggregateRoot, bool>> expression, Func<IQueryable<TAggregateRoot>, IOrderedQueryable<TAggregateRoot>>? orderBy = null)
    {
        var query = _collection.AsQueryable().Where(expression);
        return orderBy != null ? orderBy(query) : query;
    }

    public async Task<IEnumerable<TAggregateRoot>> WhereAsync(Expression<Func<TAggregateRoot, bool>>? expression, Func<IQueryable<TAggregateRoot>, IOrderedQueryable<TAggregateRoot>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = expression != null ? _collection.AsQueryable().Where(expression) : _collection.AsQueryable();
        query = orderBy != null ? orderBy(query) : query;

        var result = await _collection.FindAsync(expression);
        return result.ToEnumerable();
    }

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> expression)
    {
        var filter = Builders<TAggregateRoot>.Filter.Where(expression);
        return await _collection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<TAggregateRoot> AddAsync(TAggregateRoot entity)
    {
        var userId = _claimReader.GetUserIdFromClaim();
        entity.UpdateTrackingInformation(_username, _dateTimeProvider.UtcNow);
        entity.AddUser(userId);

        await _collection.InsertOneAsync(entity);

        return entity;
    }

    public async Task UpdateAsync(TAggregateRoot entity)
    {
        entity.UpdateTrackingInformation(_username, _dateTimeProvider.UtcNow);

        var filter = Builders<TAggregateRoot>.Filter.Eq("_id", entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    /* [TODO]: to be improved - probably not the most efficient option */
    public async Task UpdateRangeAsync(IEnumerable<TAggregateRoot> entities)
    {
        var bulkOperations = new List<WriteModel<TAggregateRoot>>();

        foreach (var entity in entities)
        {
            entity.UpdateTrackingInformation(_username, _dateTimeProvider.UtcNow);

            var filter = Builders<TAggregateRoot>.Filter.Eq("_id", entity.Id); // Assuming _id is the identifier field

            var replaceOneModel = new ReplaceOneModel<TAggregateRoot>(filter, entity)
            {
                IsUpsert = false // Set to true if you want to upsert
            };

            bulkOperations.Add(replaceOneModel);
        }

        var options = new BulkWriteOptions { IsOrdered = false }; // Set IsOrdered as needed

        if (bulkOperations.Any())
        {
            await _collection.BulkWriteAsync(bulkOperations, options);
        }
    }

    public async Task DeleteAsync(TAggregateRoot entity)
    {
        var filter = Builders<TAggregateRoot>.Filter.Eq("_id", entity.Id);
        await _collection.DeleteOneAsync(filter);
    }
}