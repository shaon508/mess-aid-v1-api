using Hangfire;
using MessAidVOne.API.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

builder.AddAuthenticationServices();

builder.Services.AddSwaggerDocumentation();
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

//builder.Services.Configure<CloudinarySettings>(
//    builder.Configuration.GetSection("Cloudinary"));

//builder.Services.AddScoped<CloudinaryService>();


var app = builder.Build();



app.UseSwaggerDocumentation();

app.UseHangfireDashboard();
app.RegisterHangfireJobs();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();