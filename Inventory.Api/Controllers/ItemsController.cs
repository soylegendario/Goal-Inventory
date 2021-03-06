using FluentValidation;
using Inventory.Application.Contracts;
using Inventory.Application.Dto;
using Inventory.Infrastructure.Exceptions;
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
        try
        {
            await _itemWriteService.AddItem(item);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ValidationException e)
        {
            _logger.LogError(e, "POST: /items, validation error");
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete]
    [Authorize]
    [Route("/{name}")]
    [SwaggerOperation("Delete a item by name")]
    [SwaggerResponse(200, Description = "Operation success")]
    [SwaggerResponse(400, Description = "Bad request")]
    [SwaggerResponse(401, Description = "Unauthorized")]
    [SwaggerResponse(404, Description = "Not found")]
    [SwaggerResponse(500, Description = "Unexpected error")]
    public async Task<IActionResult> Delete(string name)
    {
        try
        {
            _logger.LogInformation("DELETE: /items");
            await _itemWriteService.RemoveItemByName(name);
            return Ok();
        }
        catch (ItemNotFoundException e)
        {
            _logger.LogError(e, "Item not found");
            return NotFound();
        }
    }
}