using Microsoft.EntityFrameworkCore;
using ListingService.Models;

namespace ListingService.Data
{
    public class ListingDbContext : DbContext
    {
        public ListingDbContext(DbContextOptions<ListingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Listing> Listings { get; set; }  // DbSet för Listing-entiteter
    }
}
