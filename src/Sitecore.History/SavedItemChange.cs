using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SitecoreHistory
{
    [BsonIgnoreExtraElements]
    public class SavedItemChange:ItemChange
    {
        [BsonId]
        public ObjectId _id { get; set; }
    }
}