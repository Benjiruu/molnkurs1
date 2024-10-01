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
            messageService.SendLoggingActions("Ad: " + listing.Title + " created by userID:" + listing.UserId);
            return CreatedAtAction("PostListing", new { id = listing.Id }, listing);
        }

        // DELETE: api/listing/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListing(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound(new { message = "Listing not found." });
            }

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();

            
            messageService.NotifyListingUpdate(id.ToString());
            messageService.SendLoggingActions("Ad: " + listing.Title + " deleted by userID:" + listing.UserId);

            return Ok(new { message = $"Listing {id} deleted successfully." });
        }
    }
}

public class ListingDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required string UserId { get; set; }
    public required DateTime CreatedAt { get; set; }
}
