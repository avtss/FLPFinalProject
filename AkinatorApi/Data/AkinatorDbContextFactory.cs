using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AkinatorApi.Data;

public class AkinatorDbContextFactory : IDesignTimeDbContextFactory<AkinatorDbContext>
{
	public AkinatorDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<AkinatorDbContext>();
		optionsBuilder.UseNpgsql("Host=localhost;Database=Akinator;Username=postgres;Password=12345");

		return new AkinatorDbContext(optionsBuilder.Options);
	}
}
