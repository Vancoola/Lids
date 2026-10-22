using Application.Endpoints;
using Application.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapAuthEndpoints();
app.MapHealthChecks("/health");

app.Run();