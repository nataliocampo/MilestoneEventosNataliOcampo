using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MilestoneEventosNataliOcampo.data.Db;
using MilestoneEventosNataliOcampo.data.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme =
   CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
 .AddCookie()
 .AddGoogle(GoogleDefaults.AuthenticationScheme, options => {
     options.ClientId =
    builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
     options.ClientSecret =
    builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
 });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbTurnoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();
var app = builder.Build();




//aplicar en nueva bd migraciones
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbTurnoContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Turnos}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
