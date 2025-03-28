using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tlg_bot.Models;
using MongoDB.Driver;

namespace tlg_bot
{
    public class MongoService
    {
        private readonly IMongoCollection<Auditorium> _auditoriums;
        private readonly IMongoCollection<Stats> _stats;

        public MongoService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("nure_bot_db");

            _auditoriums = db.GetCollection<Auditorium>("auditoriums");
            _stats = db.GetCollection<Stats>("stats");

            if (_stats.CountDocuments(Builders<Stats>.Filter.Empty) == 0)
                _stats.InsertOne(new Stats { TotalUsers = 0, SearchCount = 0 });
        }

        public async Task<List<string>> GetSubjects()
        {
            return await _auditoriums.Distinct<string>("subject", FilterDefinition<Auditorium>.Empty).ToListAsync();
        }

        public async Task<Auditorium> FindByNumber(string number) =>
            await _auditoriums.Find(a => a.Number == number).FirstOrDefaultAsync();

        public async Task<Auditorium> FindBySubject(string subject) =>
            await _auditoriums.Find(a => a.Subject == subject).FirstOrDefaultAsync();

        public async Task IncrementSearchCount() =>
            await _stats.UpdateOneAsync(Builders<Stats>.Filter.Empty, Builders<Stats>.Update.Inc("SearchCount", 1));

        public async Task AddUniqueUser(long userId)
        {
            var filter = Builders<Stats>.Filter.And(
                Builders<Stats>.Filter.Empty,
                Builders<Stats>.Filter.Not(Builders<Stats>.Filter.AnyEq("UniqueUserIds", userId))
            );

            var update = Builders<Stats>.Update
                .Inc("TotalUsers", 1)
                .AddToSet("UniqueUserIds", userId);

            await _stats.UpdateOneAsync(filter, update);
        }

        public async Task<Stats> GetStats() =>
            await _stats.Find(_ => true).FirstOrDefaultAsync();

        public async Task IncrementAuditoriumSearch(string number)
        {
            var filter = Builders<Auditorium>.Filter.Eq(a => a.Number, number);
            var update = Builders<Auditorium>.Update.Inc(a => a.SearchHits, 1);
            await _auditoriums.UpdateOneAsync(filter, update);
        }

        public async Task<Auditorium> GetMostPopularAuditorium()
        {
            return await _auditoriums.Find(_ => true)
                .SortByDescending(a => a.SearchHits)
                .Limit(1)
                .FirstOrDefaultAsync();
        }
    }
}
