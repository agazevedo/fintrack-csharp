using Fintrack.API.Data;
using Fintrack.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
		Name = "Authorization",
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Description = "Enter JWT token"
	});

	options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { {
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
				Reference = new Microsoft.OpenApi.Models.OpenApiReference {
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}}
	);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseMySql(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		ServerVersion.AutoDetect(
			builder.Configuration.GetConnectionString("DefaultConnection")
		)
	);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
	var key = Encoding.UTF8.GetBytes(
		builder.Configuration["Jwt:Key"]!
	);

	options.TokenValidationParameters =
		new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],

			IssuerSigningKey =
				new SymmetricSecurityKey(key)
		};
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options => {
	options.AddPolicy("AllowReact",
		policy =>
		{
			policy.AllowAnyOrigin()
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});

builder.Services.AddScoped<JwtService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowReact");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
