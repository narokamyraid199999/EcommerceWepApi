namespace EcommerceWepApi.DTO
{
	public class OrderDto
	{
		public int Id { get; set; }

		public List<productDTo> Products { get; set; } = new List<productDTo>();

		public int numberOfItems { get; set; }

		public double TotalPrice { get; set; }

		public string status { get; set; } = "success";
	}
}
