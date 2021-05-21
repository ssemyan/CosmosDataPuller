using System;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CosmosDataPuller
{
    public static class DataFiller
    {

        [FunctionName("DataFiller")]
        public static async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer,
             [CosmosDB(
                databaseName: "%CosmosDatabaseName%",
                collectionName: "%CosmosCollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<SensorData> cosmosOut, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            int nRowCount = 0;
            // Create 3 new readings for each customer
            for (int i = 0; i < 2; i++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var newReading = new SensorData
                    {
                        id = Guid.NewGuid().ToString(),
                        customerid = "cust" + (i + 1),
                        sensorid = "sensor" + (x + 1),
                        timestamp = DateTime.Now,
                        timeid = DateTime.Now.Ticks,
                        data = "Sample data"
                    };
                    await cosmosOut.AddAsync(newReading);
                    nRowCount++;
                }
            }
            log.LogInformation($"Sent {nRowCount} rows to Cosmos collection {Environment.GetEnvironmentVariable("CosmosCollectionName")}");
        }
    }
}
