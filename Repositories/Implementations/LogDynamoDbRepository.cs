using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using System.Text.Json;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class LogDynamoDbRepository : ILogRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _logsTable;

        public LogDynamoDbRepository(IAmazonDynamoDB dynamoDb, IConfiguration configuration)
        {
            _dynamoDb = dynamoDb;
            _logsTable = configuration["AWS:LogsTable"] ?? throw new ArgumentNullException("AWS:LogsTable not configured");
        }

        // ------------------------------------------------------------
        // ADD LOG
        // ------------------------------------------------------------
        public async Task AddLogAsync(Log log)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                ["PK"] = new AttributeValue { S = log.PK },
                ["SK"] = new AttributeValue { S = log.SK },
                ["Username"] = new AttributeValue { S = log.Username },
                ["Action"] = new AttributeValue { S = log.Action },
                ["EntityType"] = new AttributeValue { S = log.EntityType },
                ["EntityId"] = new AttributeValue { N = log.EntityId.ToString() }
            };

            if (log.Metadata != null && log.Metadata.Any())
            {
                // Store Metadata as JSON string
                string metadataJson = JsonSerializer.Serialize(log.Metadata);
                item["Metadata"] = new AttributeValue { S = metadataJson };
            }

            var request = new PutItemRequest
            {
                TableName = _logsTable,
                Item = item
            };

            await _dynamoDb.PutItemAsync(request);
        }

        // ------------------------------------------------------------
        // GET LOGS BY ENTITY TYPE + ENTITY ID
        // ------------------------------------------------------------
        public async Task<List<Log>> GetLogsAsync(string entityType, int entityId)
        {
            string pk = $"{entityType}#{entityId}";

            var request = new QueryRequest
            {
                TableName = _logsTable,
                KeyConditionExpression = "PK = :v_pk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":v_pk"] = new AttributeValue { S = pk }
                },
                ScanIndexForward = true // chronological order
            };

            var response = await _dynamoDb.QueryAsync(request);

            var logs = response.Items.Select(item => new Log
            {
                PK = item["PK"].S,
                SK = item["SK"].S,
                Username = item["Username"].S,
                Action = item["Action"].S,
                EntityType = item["EntityType"].S,
                EntityId = int.Parse(item["EntityId"].N),
                Metadata = item.ContainsKey("Metadata")
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(item["Metadata"].S)
                    : new Dictionary<string, object>()
            }).ToList();

            return logs;
        }
    }
}
