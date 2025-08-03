using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Extensions;
using UserManagementApp.Middleware;
using UserManagementApp.Models;
using UserManagementApp.Services;
using UserManagementApp.Validators;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentity();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddFluentValidators();

builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<UserStatusCheckMiddleware>();

app.UseRouting();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
