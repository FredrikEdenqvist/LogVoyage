# LogVoyage
## What it is
a demo environment for getting cozy with structured logging in dotnet with filebeat, elasticsearch and kibana. This demo environment runs on Docker Desktop (works on mac and on windows from powershell/cmd **not WSL**)
## How to run

* run ```docker-compose up``` (in powershell or cmd on windows)
* navigate to localhost:5601
* in Kibana (menu) => discover => add index ```filebeat-*```  (click *create data view* and add *filebeat-** as index pattern and click Save data view to Kibana) 
* search for ```container.name: app and data.Position.Latitude: *```

## How it works

This docker-compose setup spins up four containers: Elasticsearch, Filebeat, Kibana, and App.

* app: A C# application utilizing Serilog to log entries to stdout (console).
* filebeat: Monitors and captures the application logs, forwarding them to Elasticsearch.
* elasticsearch: Stores the logs received.
* kibana: Provides a user interface to visualize and query the stored logs, including the structured JSON data from certain entries.

### The Structured Logging part 

Serilog allows for object destructuring into JSON. For example, the "Position" in this entry: 
```
var position = new { Latitude = 25, Longitude = 134 };
log.Information("Processed {@Position} in {Elapsed} ms", position, elapsedMs);
```
Filebeat is configured with a pre-processor that parses the JSON content from the logs and maps them to a target named data. This configuration can be observed in filebeat.yaml under:
```
- decode_json_fields:
      fields: ["message"]
      target: "data"
      overwrite_keys: true
```

In Kibana, the log entries will have properties prefixed with data.*, determined by the objects present in the log entry.
