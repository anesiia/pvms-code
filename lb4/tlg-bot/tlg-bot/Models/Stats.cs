using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tlg_bot.Models
{
    public class Stats
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public int TotalUsers { get; set; }
        public int SearchCount { get; set; }

        public List<long> UniqueUserIds { get; set; } = new List<long>();
    }
}
