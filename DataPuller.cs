using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Collections.Generic;

namespace CosmosDataPuller
{
    public static class DataPuller
    {
        const int PAGE_SIZE = 10;

        [FunctionName("DataPuller")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDatabaseName%",
                collectionName: "%CosmosCollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient cosmosClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string custId = req.Query["custid"];
            string timeidStr = req.Query["timeid"];
            long timeid;
            if (! long.TryParse(timeidStr, out timeid) || string.IsNullOrEmpty(custId))
            {
                return new BadRequestObjectResult("Must pass valid custid and timeid in URL");
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Environment.GetEnvironmentVariable("CosmosDatabaseName"), Environment.GetEnvironmentVariable("CosmosCollectionName"));
            log.LogInformation($"Getting the first {PAGE_SIZE} rows of sensor data for customer {custId} with timeid > {timeid}");

            IDocumentQuery<SensorData> query = cosmosClient.CreateDocumentQuery<SensorData>(collectionUri)
                .Where(p => p.customerid == custId && p.timeid > timeid)
                .OrderBy(p => p.timeid)
                .Take(PAGE_SIZE)
                .AsDocumentQuery();

            List<SensorData> retData = new List<SensorData>();
            while (query.HasMoreResults)
            {
                foreach (SensorData result in await query.ExecuteNextAsync())
                {
                    retData.Add(result);
                }
            }
            log.LogInformation($"Returning {retData.Count} records.");

            var retObj = new SensorDataItems { SensorData = retData.ToArray() };
            
            // Add the last timeid to the returned object so the caller can use it to get more data (or just the passed in timeid if no records found)
            retObj.LastTimeId = retObj.SensorData.Length == 0 ? timeid : retObj.SensorData[retObj.SensorData.Length - 1].timeid;

            return new OkObjectResult(retObj);
        }

        class SensorDataItems
        {
            public SensorData[] SensorData { get; set; }
            public long LastTimeId { get; set; }

        }
    }

}
