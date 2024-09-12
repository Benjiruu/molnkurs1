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

        public ListingController(ListingDbContext context)
        {
            _context = context;
        }

        // POST: api/listing
        [HttpPost]
        public async Task<IActionResult> PostListing([FromBody] Listing listing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            listing.Id = 0;

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostListing", new { id = listing.Id }, listing);
        }
    }
}
