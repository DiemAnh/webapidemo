using Microsoft.AspNetCore.Mvc;
using WebAPIDemo.Models;
using WebAPIDemo.Services;

namespace WebAPIDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorDatasController : ControllerBase
{
    private readonly SensorDataService _service;

    public SensorDatasController(SensorDataService service)
    {
        _service = service;
    }

    [HttpGet("/api/AppVersion")]
    public IActionResult GetAppVersion()
    {
        return Ok("Version 1.0");
    }

    [HttpPost("/api/PostVersion")]
    public IActionResult PostVersion([FromBody] BodyData body)
    {
        Console.WriteLine($"DeviceId: {body.DeviceId}");
        Console.WriteLine($"Version: {body.Version}");

        return Ok("Received");
    }

    // GET: api/SensorDatas
    [HttpGet]
    public async Task<List<SensorData>> Get()
    {
        return await _service.GetAsync();
    }

    // GET: api/SensorDatas/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SensorData>> Get(string id)
    {
        var sensor = await _service.GetAsync(id);

        if (sensor == null)
            return NotFound();

        return sensor;
    }

    // POST: api/SensorDatas
    [HttpPost]
    public async Task<IActionResult> Post(SensorData sensorData)
    {
        await _service.CreateAsync(sensorData);

        return CreatedAtAction(nameof(Get),
            new { id = sensorData.Id },
            sensorData);
    }

    // PUT: api/SensorDatas/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, SensorData sensorData)
    {
        var oldData = await _service.GetAsync(id);

        if (oldData == null)
            return NotFound();

        sensorData.Id = id;

        await _service.UpdateAsync(id, sensorData);

        return NoContent();
    }

    // DELETE: api/SensorDatas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var sensor = await _service.GetAsync(id);

        if (sensor == null)
            return NotFound();

        await _service.DeleteAsync(id);

        return NoContent();
    }
}