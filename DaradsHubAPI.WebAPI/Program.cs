using DaradsHubAPI.Core;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared;
using DaradsWebMobAPIs.WebAPI.Extensions;
using DaradsWebMobAPIs.WebAPI.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("mycon")!));

builder.Services.AddWebServices(builder.Configuration);
builder.Services.AddControllers();
CoreBootstrapper.InitServices(builder.Services);
SharedBootstrapper.InitServices(builder.Services);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseStatusCodePages();
app.UseAuthorization();
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller}/{action}/{id?}");

app.MapAreaControllerRoute(
    name: "CustomerArea",
    areaName: "Customer",
    pattern: "Customer/{controller}/{action}/{id?}");

app.MapAreaControllerRoute(
    name: "AgentArea",
    areaName: "Agent",
    pattern: "Agent/{controller}/{action}/{id?}");

app.MapControllers();

app.Run();
