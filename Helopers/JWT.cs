namespace EcommerceWepApi.Helopers
{
	public class JWT
	{
        public string key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DurationInDays { get; set; }
	}
}
