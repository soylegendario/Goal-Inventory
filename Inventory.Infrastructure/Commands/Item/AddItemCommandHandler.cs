using Inventory.CrossCutting.Cqrs.Commands;
using Inventory.CrossCutting.Data;
using Inventory.Domain.Items;
using Inventory.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Commands;

internal class AddItemCommandHandler : ICommandHandler<AddItemCommand>
{
    private readonly ILogger<AddItemCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AddItemCommandHandler(ILogger<AddItemCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public Task HandleAsync(AddItemCommand command, CancellationToken cancellation = default)
    {
        _logger.LogInformation("Adding item with Name: {Name} and ExpirationDate {ExpirationDate}",
            command.Item.Name,
            command.Item.ExpirationDate);
        _unitOfWork.Repository<IItemRepository>().AddItem(command.Item);
        return _unitOfWork.SaveChangesAsync();
    }
}