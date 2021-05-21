# CosmosDataPuller
Azure Function to pull data from Azure CosmosDB using a time stamp as a place marker in the chronological data

# Setup
1. Create a new CosmosDB account in Azure (or use the [CosmosDB Local Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator))
1. Create a new container in Cosmos and set the **partition key** to */customerid*
1. Edit the *local.settings.json* file and set CosmosDatabaseName, CosmosCollectionName, and CosmosDBConnection to match your settings. 

# Running
This Azure Functions project has two functions:
1. DataFiller - this function runs every 30 seconds and will add 3 sensor settings each to customer ids cust1 and cust2.
1. DataPuller - given a customer id (custid) and timestamp (timeid) it will pull the first 10 records from Cosmos that match the customer ID and where the timestamp is greater than timeid.
The returned object is the array of sensor data values and a value representing the last timestamp retrieved. This timestamp can be used to retrieve the next set of records, and so on until no records are returned. 

# Sample usage
A python3 script *sampleUsage.py* is included to show how the function might be called and how to use the timestamp returned to retrieve the next set of records. 

