using ChatApp.Data;
using ChatApp.JWTTOEkn.Rclass;
using ChatApp.Models;
using ChatApp.Repository;
using ChatApp.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


 builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Configuration.AddUserSecrets<Program>();
var aesKeyB64 = builder.Configuration["AESKey"];
builder.Services.AddSingleton<IAesGcmEncryptionService>(
    _ => new AesGcmEncryptionService(aesKeyB64));

//builder.Services.AddScoped<IAesGcmEncryptionService>(
//    _ => new AesGcmEncryptionService(aesKeyB64)
//);
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher<ChatApp.Models.User>,PasswordHasher<ChatApp.Models.User>>();


builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddSignalR();

builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
    {
        var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
        context.Response.Redirect($"/Auth/Login?returnUrl={returnUrl}");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");
app.Run();
