using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace insert_todays_sliding_window_schedule;

public class ManualRun
{
    private readonly ILogger<ManualRun> _logger;

    public ManualRun(ILogger<ManualRun> logger)
    {
        _logger = logger;
    }

    [Function("ManualRun")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route ="v1/manualrun")] HttpRequest req)
    {
        return StartLinensDay.Start();
    }
}
