using Amazon.DynamoDBv2.DataModel;

namespace AutoPartInventorySystem.Models
{
    [DynamoDBTable("Logs")]
    public class Log
    {
        // PK: EntityType#EntityId (ex: "AutoPart#52", "User#10")
        [DynamoDBHashKey("PK")]
        public string PK { get; set; } = string.Empty;

        // SK: Timestamp (ISO8601)
        [DynamoDBRangeKey("SK")]
        public string SK { get; set; } = string.Empty;

        // Who performed the action
        public string Username { get; set; } = string.Empty;

        // Type of action (Created, Deleted, Updated, LinkedVehicle, Login, etc.)
        public string Action { get; set; } = string.Empty;

        // Entity type: AutoPart, User, Brand, Category, Vehicle...
        public string EntityType { get; set; } = string.Empty;

        // The ID of the entity
        public int EntityId { get; set; }

        // Extra dynamic fields depending on scenario
        // (Example: ImageUrl, UpdatedFields, BrandIds, Before/After, etc.)
        public Dictionary<string, object>? Metadata { get; set; } = new();
    }
}
