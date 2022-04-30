using Inventory.Domain.Items;
using Inventory.Infrastructure.Exceptions;
using Inventory.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Repository;

public class ItemInMemoryRepository : IItemRepository
{
    private readonly ILogger<ItemInMemoryRepository> _logger;
    private readonly InventoryInMemoryContext _context;

    public ItemInMemoryRepository(ILogger<ItemInMemoryRepository> logger, InventoryInMemoryContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void AddItem(Item item)
    {
        try
        {
            _logger.LogInformation("Adding item to database");
            item.Id = Guid.NewGuid();
            _context.Items.Add(item);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding item to database");
            throw;
        }
    }

    public IEnumerable<Item> GetAllItems()
    {
        try
        {
            return _context.Items;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting all items from database");
            throw;
        }
    }

    public void RemoveItemByName(string name)
    {
        try
        {
            _logger.LogInformation("Removing item with name {Name} from database", name);
            var item = _context.Items.FirstOrDefault(i => i.Name == name);
            if (item is null)
            {
                throw new ItemNotFoundException($"Item with name {name} not found");
            }

            _context.Items.Remove(item);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing item from database");
            throw;
        }
    }
}