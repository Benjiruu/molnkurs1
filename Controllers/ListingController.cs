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
        public async Task<IActionResult> PostListing([FromBody] ListingDto listing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var listing1 = new Listing
            {
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                CreatedAt = DateTime.Now,
            };

            _context.Listings.Add(listing1);
            await _context.SaveChangesAsync();
            messageService.NotifyListingCreation(listing1); 
            messageService.SendLoggingActions("Ad: " + listing.Title + " created");
            return CreatedAtAction("PostListing", new { listing1.Id }, listing1);
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
            messageService.SendLoggingActions("Ad: " + listing.Title + " deleted");

            return Ok(new { message = $"Listing {id} deleted successfully." });
        }
    }
}

public class ListingDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
}
