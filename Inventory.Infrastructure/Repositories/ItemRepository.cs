﻿using Inventory.CrossCutting.Data;
using Inventory.CrossCutting.Exceptions;
using Inventory.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class ItemRepository : BaseRepository, IItemRepository
{
    private DbSet<Item> Items => Context.Set<Item>();
    
    public void AddItem(Item item)
    {
        Context.Add(item);
    }

    public IEnumerable<Item> GetAllItems()
    {
        return Items.ToArray();
    }

    public IEnumerable<Item> GetItemsByExpirationDate(DateTime expirationDate)
    {
        return Items.Where(i => i.ExpirationDate.Date == expirationDate.Date);
    }

    public void RemoveItemByName(string name)
    {
        var item = Items.FirstOrDefault(i => i.Name == name);
        if (item is null)
        {
            throw new ItemNotFoundException($"Item with name {name} not found");
        }

        Context.Remove(item);
    }

    public void UpdateItem(Item item)
    {
        Context.Update(item);
    }
}