using ListingService.Data;
using ListingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly ListingDbContext _context;
        private readonly MessageService messageService;

        public ListingController(ListingDbContext context,MessageService messageService)
        {
            _context = context;
            this.messageService = messageService;
        }

        // POST: api/listing
        [HttpPost]
        public async Task<IActionResult> PostListing([FromBody] Listing listing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var listing1 = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                UserId = listing.UserId,
                CreatedAt = DateTime.Now,
            };

            listing.Id = 0;

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
            messageService.NotifyListingCreation(listing1); 
            return CreatedAtAction("PostListing", new { id = listing.Id }, listing);
        }
    }
}

public class ListingDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

