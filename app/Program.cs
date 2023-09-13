using Serilog;
using Serilog.Formatting.Compact;

var log = new LoggerConfiguration()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();


var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
while (await timer.WaitForNextTickAsync())
{
    log.Information("Hello, Serilog! this entry has no data.");
    var position = new { Latitude = 25, Longitude = 134 };
    var elapsedMs = 34;
    log.Information("Processed {@Position} in {Elapsed} ms", position, elapsedMs);

    var context = new { Id = 1, Name = "Foo" };
    log.Warning("Warning warning, please see context {@Context}", context);

    var exception = new Exception("Boom!");
    log.Error(exception, "Exception thrown");
}



