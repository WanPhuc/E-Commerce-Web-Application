using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Data.Seeders;

var backendRoot = FindBackendRoot();
LoadEnv(Path.Combine(backendRoot, ".env"));

var connectionString = GetArgValue(args, "--connection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__PostgresConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings:PostgresConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("Missing PostgreSQL connection string.");
    Console.Error.WriteLine("Set ConnectionStrings__PostgresConnection in web_banhang_be/.env or pass --connection \"...\".");
    return 1;
}

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(connectionString)
    .Options;

await using var db = new AppDbContext(options);

Console.WriteLine("Ensuring database schema...");
await db.Database.EnsureCreatedAsync();

Console.WriteLine("Seeding base roles/admin...");
await RoleSeeder.SeedAsync(db);
await UserSeeder.SeedAsync(db);

Console.WriteLine("Seeding demo data...");
await DemoDataSeeder.SeedAsync(db);

Console.WriteLine("Demo seed completed.");
Console.WriteLine("Seller: seller.demo@shoppy.local / demo123456");
Console.WriteLine("Buyer : buyer.demo@shoppy.local / demo123456");
return 0;

static string? GetArgValue(string[] args, string name)
{
    var index = Array.IndexOf(args, name);
    if (index < 0 || index + 1 >= args.Length)
    {
        return null;
    }

    return args[index + 1];
}

static string FindBackendRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory != null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "WebBanHang.csproj")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new DirectoryNotFoundException("Cannot find backend root containing WebBanHang.csproj.");
}

static void LoadEnv(string envPath)
{
    if (!File.Exists(envPath))
    {
        return;
    }

    foreach (var rawLine in File.ReadAllLines(envPath))
    {
        var line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith("#"))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();

        if (Environment.GetEnvironmentVariable(key) == null)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

