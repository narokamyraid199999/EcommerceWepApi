using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.DTO
{
    public class WishlistDto
    {
        public int Id { get; set; }

        public List<productDTo> Products { get; set; } = new List<productDTo>();

        public int numberOfItems { get; set; }

        public double TotalPrice { get; set; }

        public string status { get; set; } = "success";

    }
}
