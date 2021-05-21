using System;

namespace CosmosDataPuller
{
    public class SensorData
    { 
        public string id { get; set; }
        public string customerid { get; set; }
        public long timeid { get; set; }
        public string sensorid { get; set; }
        public DateTime timestamp { get; set; }
        public string data { get; set; }
    }
}
