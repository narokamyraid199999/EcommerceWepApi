namespace EcommerceWepApi.DTO
{
	public class AuthDto
	{
        public string Username { get; set; }
		
        public string Email { get; set; }
        
        public bool isAuthenticated { get; set; }
        
        public string Token { get; set; }
        
        public List<string> Roles { get; set; }
        
        public DateTime ExpireDate { get; set; }
    }
}
