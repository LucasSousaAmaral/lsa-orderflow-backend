using LSA.OrderFlow.Application.Orders.Commands.CreateOrder;
using LSA.OrderFlow.Application.Orders.Commands.DeleteOrder;
using LSA.OrderFlow.Application.Orders.Commands.UpdateOrder;
using LSA.OrderFlow.Application.Orders.Queries.GetOrderById;
using LSA.OrderFlow.Application.Orders.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LSA.OrderFlow.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public Task<OrderDetailsVm> GetById(Guid id) =>
        _mediator.Send(new GetOrderByIdQuery(id));

    [HttpGet]
    public Task<PagedList<OrderListItemVm>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null) =>
        _mediator.Send(new ListOrdersQuery(page, pageSize, search));

    [HttpPut("{id:guid}")]
    public Task<Unit> Update(Guid id, [FromBody] UpdateOrderCommand cmd) =>
        _mediator.Send(cmd with { OrderId = id });

    [HttpDelete("{id:guid}")]
    public Task<Unit> Delete(Guid id) =>
        _mediator.Send(new DeleteOrderCommand(id));
}
