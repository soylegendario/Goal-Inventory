using Inventory.Application.Contracts;
using Inventory.Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> _logger;
    private readonly IItemReadService _itemReadService;
    private readonly IItemWriteService _itemWriteService;

    public ItemsController(ILogger<ItemsController> logger, IItemReadService itemReadService, IItemWriteService itemWriteService)
    {
        _logger = logger;
        _itemReadService = itemReadService;
        _itemWriteService = itemWriteService;
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation("Get all items")]
    [SwaggerResponse(200, Description = "Operation success", Type = typeof(IEnumerable<ItemDto>))]
    [SwaggerResponse(401, Description = "Unauthorized")]
    [SwaggerResponse(500, Description = "Unexpected error")]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("GET: /items");
        var items = await _itemReadService.GetAllItems();
        return Ok(items);
    }
    
    [HttpPost]
    [Authorize]
    [SwaggerOperation("Create a new item")]
    [SwaggerResponse(201, Description = "Created")]
    [SwaggerResponse(400, Description = "Bad request")]
    [SwaggerResponse(401, Description = "Unauthorized")]
    [SwaggerResponse(500, Description = "Unexpected error")]
    public async Task<IActionResult> Post([FromBody] ItemDto item)
    {
        _logger.LogInformation("POST: /items");
        await _itemWriteService.AddItemAsync(item);
        return StatusCode(StatusCodes.Status201Created);
    }
    
    [HttpDelete("{name}")]
    [Authorize]
    [SwaggerOperation("Delete a item by name")]
    [SwaggerResponse(200, Description = "Operation success")]
    [SwaggerResponse(400, Description = "Bad request")]
    [SwaggerResponse(401, Description = "Unauthorized")]
    [SwaggerResponse(404, Description = "Not found")]
    [SwaggerResponse(500, Description = "Unexpected error")]
    public async Task<IActionResult> Delete(string name)
    {
        _logger.LogInformation("DELETE: /items");
        await _itemWriteService.RemoveItemByNameAsync(name);
        return Ok();
    }

    [HttpPut]
    [Authorize]
    [SwaggerOperation("Update a item")]
    [SwaggerResponse(204, Description = "Operation success")]
    [SwaggerResponse(400, Description = "Bad request")]
    [SwaggerResponse(401, Description = "Unauthorized")]
    [SwaggerResponse(404, Description = "Not found")]
    [SwaggerResponse(500, Description = "Unexpected error")]
    public async Task<IActionResult> Update([FromBody] ItemDto item)
    {        
        _logger.LogInformation("PUT: /items");
        await _itemWriteService.UpdateItemAsync(item);
        return StatusCode(StatusCodes.Status204NoContent);
    }
}