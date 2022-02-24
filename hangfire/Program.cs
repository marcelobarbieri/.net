using Hangfire;
using Hangfire.Storage.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Hangfire -------------------------------------------------------------------

builder.Services
    .AddHangfire(configuration => configuration
        .UseRecommendedSerializerSettings()
        .UseSQLiteStorage());

// Define a quantidade de retentativas aplicadas a um job com falha.
// Por padrão serão 10, aqui estamos abaixando para duas com intervalo de 5 minutos.
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
{
    Attempts = 5,
    DelaysInSeconds = new int[] { 300 }
});

builder.Services.AddHangfireServer();

// ------------------------------------------------------------------- Hangfire ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// --- Hangfire -------------------------------------------------------------------

app.UseHangfireDashboard();

var jobId1 = BackgroundJob.Enqueue(
    () => Console.WriteLine("Job Fire-and-forget!"));

BackgroundJob.Schedule(
    () => Console.WriteLine("Job Delayed: 2 minutos após o início da aplicação"),
    TimeSpan.FromMinutes(2));

var jobId2 = BackgroundJob.Enqueue(() => Console.WriteLine("Job fire-and-forget pai!"));
BackgroundJob.ContinueJobWith(
    jobId2,
    () => Console.WriteLine($"Job continuation! (Job pai: {jobId2})"));

RecurringJob.AddOrUpdate(
    "Meu job recorrente",
    () => Console.WriteLine((new Random().Next(1, 200) % 2 == 0)
        ? "Job recorrente gerou um número par"
        : "Job recorrente gerou um número ímpar"),
    Cron.Minutely,
    TimeZoneInfo.Local);

// ------------------------------------------------------------------- Hangfire ---

app.Run();
