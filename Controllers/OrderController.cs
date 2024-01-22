using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    public class OrderController : ControllerBase
    {
        private readonly IListRepository<Order> _orderRepository;

        public OrderController(IListRepository<Order> orderRepo)
        {
            _orderRepository = orderRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Order> orders = _orderRepository.GetAll();
            return Ok(orders);
        }

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var orders = _orderRepository.GetById(userId);
            if (orders == null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult Post(Order newOrder)
        {
            bool added = _orderRepository.Add(newOrder);
            if (!added)
            {
                return BadRequest("Failed to create Order");
            }

            return Ok();
        }

        [HttpPut("{orderId}")]
        public IActionResult Put(int orderId, [FromBody] Order updatedOrder)
        {
            // Check if the provided ID matches the ID in the updatedOrder
            if (orderId != updatedOrder.OrderID)
            {
                return BadRequest("Mismatched IDs");
            }

            // Check if the order with the specified ID exists
            Order existingOrder = _orderRepository.GetObjById(orderId);
            if (existingOrder == null)
            {
                return NotFound("Order not found");
            }

            // Update the existing order properties
            existingOrder.DateTime = updatedOrder.DateTime;
            existingOrder.TotalPrice = updatedOrder.TotalPrice;
            existingOrder.Status = Enum.Parse<OrderStatus>(updatedOrder.Status.ToString());

            existingOrder.UserID = updatedOrder.UserID;

            // Perform the update in the repository
            bool updated = _orderRepository.Update(existingOrder);

            if (!updated)
            {
                return BadRequest("Failed to update order");
            }

            return Ok();
        }


        [HttpDelete("{orderId}")]
        public IActionResult Delete(int orderId)
        {
            bool deleted = _orderRepository.Delete(orderId);
            if (deleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
