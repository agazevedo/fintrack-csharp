using Fintrack.API.Data;
using Fintrack.API.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Fintrack.API.Tests.Integration;

/// <summary>
/// Sobe a API completa (pipeline, autenticação JWT, controllers) trocando o MySQL
/// por um SQLite em memória. A conexão fica aberta durante a vida da factory para
/// que o banco não seja descartado entre requisições.
/// </summary>
public class FintrackApiFactory : WebApplicationFactory<Program> {
	private readonly SqliteConnection _connection = new("DataSource=:memory:");

	protected override void ConfigureWebHost(IWebHostBuilder builder) {
		builder.UseEnvironment("Testing");

		builder.ConfigureAppConfiguration((_, config) => {
			config.AddInMemoryCollection(TestHelpers.JwtSettings);
		});

		builder.ConfigureServices(services => {
			services.RemoveAll<DbContextOptions<AppDbContext>>();

			_connection.Open();
			services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
		});
	}

	protected override IHost CreateHost(IHostBuilder builder) {
		var host = base.CreateHost(builder);

		using var scope = host.Services.CreateScope();
		scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

		return host;
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);

		if (disposing) {
			_connection.Dispose();
		}
	}
}
