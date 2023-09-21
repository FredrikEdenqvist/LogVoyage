# LogVoyage
## What it is
a demo environment for getting cozy with structured logging in dotnet with filebeat, elasticsearch and kibana. This demo environment runs on Docker Desktop (works on mac and on windows from powershell/cmd **not bash inside a WSL distro**)

## Prerequisites
* Docker Desktop (works on mac and on windows from powershell/cmd **not bash inside a WSL distro**)  
If you are running docker-desktop on windows with the WSL2 based engine elastic will likely fail to start with the following error message:  
```
"The default operating system limits on mmap counts is likely to be too low, which may result in out of memory exceptions."
``````
This is mitigated via running ```sudo sysctl -w vm.max_map_count=262144``` on wsl docker is running on. Ref. [Virtual memory (Elastic Docs)](https://www.elastic.co/guide/en/elasticsearch/reference/current/vm-max-map-count.html).

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


## Example views in Kibana
make sure you have the environment up and running and the index pattern filebeat-* created in Kibana before clicking links below

[One example view when looking at Exceptions](http://localhost:5601/app/discover#/?_g=(filters:!(),refreshInterval:(pause:!f,value:5000),time:(from:now-1d,to:now))&_a=(columns:!('data.@m','data.@l','data.@x'),filters:!(),grid:(columns:(aws.cloudwatch.message:(width:293))),index:'7bb7b3c7-7e36-4d7a-9241-3246d4e866f9',interval:auto,query:(language:kuery,query:'data.@l:%20Error'),sort:!(!('@timestamp',desc))))

[One example view when searching for a Position with Latitude greater than 24](http://localhost:5601/app/discover#/?_g=(filters:!(),refreshInterval:(pause:!f,value:5000),time:(from:now-1d,to:now))&_a=(columns:!('data.@m',data.Position.Latitude),filters:!(),grid:(columns:(aws.cloudwatch.message:(width:293))),index:'7bb7b3c7-7e36-4d7a-9241-3246d4e866f9',interval:auto,query:(language:kuery,query:'container.name:%20app%20and%20data.Position.Latitude%20%3E%2024'),sort:!(!('@timestamp',desc))))