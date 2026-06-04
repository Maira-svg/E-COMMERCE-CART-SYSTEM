using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DarazUltimateMVC.Data;
using DarazUltimateMVC.Models;
using System.Text.Json;

namespace DarazUltimateMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get cart items
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var sessionId = HttpContext.Session.Id;

            var cartItems = await _context.Carts
                .Where(c => c.UserId.ToString() == userId || c.SessionId == sessionId)
                .Include(c => c.Product)
                .ToListAsync();

            return Ok(cartItems);
        }

        // Add to cart
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var sessionId = HttpContext.Session.Id;

            var existingItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.ProductId == request.ProductId &&
                    (c.UserId.ToString() == userId || c.SessionId == sessionId));

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                var cartItem = new Cart
                {
                    SessionId = sessionId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UserId = string.IsNullOrEmpty(userId) ? null : int.Parse(userId)
                };
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Added to cart" });
        }

        // Remove from cart
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _context.Carts.FindAsync(id);
            if (item != null)
            {
                _context.Carts.Remove(item);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // Checkout - Move cart to orders
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Please login first" });
            }

            var sessionId = HttpContext.Session.Id;
            var cartItems = await _context.Carts
                .Where(c => c.UserId.ToString() == userId || c.SessionId == sessionId)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest(new { message = "Cart is empty" });
            }

            // Create order
            var order = new Order
            {
                UserId = int.Parse(userId),
                OrderNumber = "ORD" + DateTime.Now.Ticks.ToString(),
                TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),
                Status = "Pending",
                OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Title,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }

            // Clear cart
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order placed successfully!", orderId = order.Id });
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}