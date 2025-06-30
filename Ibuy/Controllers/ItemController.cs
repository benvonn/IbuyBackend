using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ibuy.Models;
using Ibuy.Data;
using Microsoft.EntityFrameworkCore;


//Add so it automatically logins in after successful signup


namespace Ibuy.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }
        //All items from all users
        [HttpPost("selling/details")]
        public async Task<IActionResult> SellItem([FromBody] ItemRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ItemName) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Price) ||
                request.UserId <= 0)
            {
                return BadRequest(new { message = "Invalid Item details" });
            }

            var newItem = new Item
            {
                ItemName = request.ItemName,
                Description = request.Description,
                Price = request.Price,
                UserId = request.UserId
            };

            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok(newItem);
        }

        [HttpGet("buylist")]
        public IActionResult ListItem()
        {
            var items = _context.Items.
                Select(i => new
                {
                    i.Id,
                    i.ItemName,
                    i.Description,
                    i.Price,
                    i.UserId,
                    i.IsSold

                })
                .ToList();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _context.Items
                .Include(i => i.User)
                .FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();

            return Ok(new
            {
                item.ItemName,
                item.Description,
                item.Price,
                SellerUsername = item.User?.Username,
                SellerContact = item.User?.PreferredContact
            });

         //User Items via profile

        }
        [HttpGet("user/{id}")]
        public IActionResult GetUserItems(int id)
        {
            var items = _context.Items
                .Where(i => i.UserId == id)
                .Select(i => new
                {
                    i.Id,
                    i.ItemName,
                    i.Description,
                    i.Price,
                    i.UserId
                    
                })
                .ToList();

            return Ok(items);
        }
        [HttpPut("{id}/mark-sold")]
        public async Task<IActionResult> MarkItemAsSold(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            item.IsSold = true;
            item.SoldAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        [HttpDelete("{id}/delete-item")]
        public async Task<IActionResult> DeleteUserItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();  

            return NoContent(); 
        }



    }
}
