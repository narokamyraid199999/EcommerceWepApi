using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class TrendingProduct
	{
		[Key]
        public int Id { get; set; }

        public bool isNew { get; set; } = false;

		public bool isFeatured { get; set; } = false;

		public bool isTopSellers { get; set; } = false;

		List<TrendingDetails> TrendingDetails { get; set;}

	}
}
