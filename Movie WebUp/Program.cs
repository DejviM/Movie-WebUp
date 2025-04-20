using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Movie_WebUp.Data;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<Movie_WebUpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Movie_WebUpContext") ?? throw new InvalidOperationException("Connection string 'Movie_WebUpContext' not found.")));

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true) // Set to false if you don’t need email confirmation
    .AddEntityFrameworkStores<Movie_WebUpContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthentication(); // Enables Authentication Middleware
app.UseAuthorization();  // Enables Authorization Middleware

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
