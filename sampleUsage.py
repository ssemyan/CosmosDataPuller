import requests
import json

custid = "cust1" # Customer to pull
timeid = "1"     # max time to pull from

base_url = "http://localhost:7071/api/DataPuller" # URL of the DataPuller

# List to save sensor data to
data = []

# Loop because API gives back prices in batches of 100
hasData = True
while (hasData):
    url = base_url + '?custid=' + custid + '&timeid=' + timeid

    # make request and parse json
    print("Calling API: " + url)
    r = requests.get(url)
    resp = json.loads(r.content)
    rowCnt = len(resp["sensorData"])
    print("Found: " + str(rowCnt) + " rows.")

    if rowCnt == 0:
        hasData = False
    else:
        # add to list
        data.extend(resp["sensorData"])

        # Get the new timeid
        timeid = str(resp["lastTimeId"])

print("Downloaded " + str(len(data)) + " rows.")

# Now we can process the list