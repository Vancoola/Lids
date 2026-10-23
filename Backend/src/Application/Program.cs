using Application.Endpoints;
using Application.Extensions;
using Data.Identity;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

builder.Services.AddDataService(builder.Configuration);
builder.Services.AddApiInfrastructure(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddDocs();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference("/docs", options =>
    {
        options.WithTitle("Lids API");
    });
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapAuthEndpoints();
app.MapHealthChecks("/health");

await app.Services.SeedSuperAdminAsync(app.Configuration);

try
{
    Log.Information("Starting Lids API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Lids API terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}