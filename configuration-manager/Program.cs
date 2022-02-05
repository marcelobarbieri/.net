using Microsoft.Extensions.Configuration.UserSecrets;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapGet("/", () => app.Configuration.GetValue<string>("Env"));

var smtp = new Configuration.SmtpConfiguration();
app.Configuration.GetSection("SmtpConfiguration").Bind(smtp);
Console.WriteLine(smtp);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine(connectionString);

app.Run();
