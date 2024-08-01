
using EcommerceWepApi.Helopers;
using EcommerceWepApi.Model;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pkix;
using System.Text;

namespace EcommerceWepApi
{
	public class Program
	{

		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<EcommerWepApiContext>(ConnectionOptions =>
			{
				ConnectionOptions.UseSqlServer(builder.Configuration.GetConnectionString("localDb"));
			});

			builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<EcommerWepApiContext>();

			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<ICategory, CategoryService>();
			builder.Services.AddScoped<IContact, ContactService>();
			builder.Services.AddScoped<IMailService, MailService>();
			builder.Services.AddScoped<ITwillioService,  TwillioService>();

			builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
			builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
			builder.Services.Configure<TwillioDTo>(builder.Configuration.GetSection("Twillio"));

			builder.Services.AddAuthentication(option =>
			{
				option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(JwtOption =>
			{
				JwtOption.RequireHttpsMetadata = false;
				JwtOption.SaveToken = false;
				JwtOption.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer= true,
					ValidateAudience= true,
					ValidateLifetime= true,
					ValidIssuer=builder.Configuration.GetSection("JWT:Issuer").Value,
					ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:key").Value))
				};
			});

			builder.Services.AddCors(option =>
			{
				option.AddPolicy("mainPolicy", policyOption => {
					policyOption.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
				});
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors("mainPolicy");

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}