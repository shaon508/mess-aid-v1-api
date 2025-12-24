using Hangfire;
using MessAidVOne.API.Extensions;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.DTOs;
using MessAidVOne.Application.Features.AuthManagement.Commands;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.AddAuthenticationServices();

builder.Services.AddSwaggerDocumentation();
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();


var tempProvider = builder.Services.BuildServiceProvider();

var handler = tempProvider.GetService<
    ICommandHandler<LogInCommand, Result<LoginDto>>>();

Console.WriteLine(handler is null
    ? "❌ Scrutor NOT working"
    : "✅ Scrutor WORKING");

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseHangfireDashboard();
app.RegisterHangfireJobs();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();