using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection.Emit;

namespace EcommerceWepApi.Model
{
	public class EcommerWepApiContext:IdentityDbContext
	{

		public EcommerWepApiContext(DbContextOptions options):base(options) 
		{ }

		public DbSet<Product> Products { get; set; }

		public DbSet<Category> Categories { get; set; }

		public DbSet<Contact> Contacts { get; set; }

		public DbSet<TrendingProduct> TrendingProducts { get; set; }

		public DbSet<TrendingDetails> TrendingDetails { get; set; }

		public DbSet<Cart> Carts { get; set; }

		public DbSet<cartDetails> cartDetailss { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

		public DbSet<wishDetails> WishDetails { get; set; }

		public DbSet<Order> orders { get; set; }

		public DbSet<OrderDetails> ordersDetails { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Product>()
			.Property(e => e.Imgs)
			.HasConversion(
				v => string.Join(',', v),
				v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
		}

	}
}
