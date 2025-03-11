using GroceryList.Infrastructure.Repositories;
using LouisManager.Api.Helpers;
using LouisManager.Api.Models;
using MongoDB.Driver;

namespace LouisManager.Api.Repositories;

public interface IEntryRepository : IRepository<Entry, Guid> {

}

public class EntryRepository : MongoDbRepositoryBase<Entry, Guid>, IEntryRepository
{
    public EntryRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider, IClaimReader claimReader) : base(database, dateTimeProvider, claimReader)
    {

    }
}