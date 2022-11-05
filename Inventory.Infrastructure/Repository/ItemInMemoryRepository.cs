using System.Runtime.CompilerServices;
using Inventory.CrossCutting.Exceptions;
using Inventory.Domain.Items;
using Inventory.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")] 
[assembly: InternalsVisibleTo("Inventory.UnitTests")]
namespace Inventory.Infrastructure.Repository;

internal class ItemInMemoryRepository : IItemRepository
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
        _logger.LogInformation("Adding item to database");
        item.Id = Guid.NewGuid();
        _context.Items.Add(item);
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _context.Items;
    }

    public IEnumerable<Item> GetItemsByExpirationDate(DateTime expirationDate)
    {
        return _context.Items.Where(i => i.ExpirationDate.Date == expirationDate.Date);
    }

    public void RemoveItemByName(string name)
    {
        _logger.LogInformation("Removing item with name {Name} from database", name);
        var item = _context.Items.FirstOrDefault(i => i.Name == name);
        if (item is null)
        {
            throw new ItemNotFoundException($"Item with name {name} not found");
        }

        _context.Items.Remove(item);
    }
}