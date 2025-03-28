using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tlg_bot.Models
{
    public class Auditorium
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("number")]
        public string Number { get; set; }

        [BsonElement("subject")]
        public string Subject { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("search_hits")]
        public int SearchHits { get; set; } = 0;
    }
}
