using FlightReservation.MVC.Context;
using FlightReservation.MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Resources;
using FlightReservation.MVC.Controllers;
using System.Reflection;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using FlightReservation.MVC.Services;
using FlightReservation.MVC.Repositories;


//A�a��daki kod blo�unda dil deste�i eklemeye �al���yoruz.
#region DI


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UserRepository>();//Her requestte dependency injection ile istendi�inde instance t�ret.
builder.Services.AddScoped<PlaneRepository>();
builder.Services.AddScoped<RouteRepository>();
builder.Services.AddScoped<TicketRepository>();

#region Localization
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
	.AddViewLocalization()
	.AddDataAnnotationsLocalization(options =>
	{
		options.DataAnnotationLocalizerProvider = (type, factory) =>
		{
			var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName ?? "");
			return factory.Create(nameof(SharedResource), assemblyName.Name ?? "");
		};
	});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportCultures = new List<CultureInfo>
	{
		new CultureInfo("en-US"),
		new CultureInfo("tr-TR")
	};

	options.DefaultRequestCulture = new RequestCulture(culture: "tr-TR", uiCulture: "tr-TR");
	options.SupportedCultures = supportCultures;
	options.SupportedUICultures = supportCultures;
	options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});

#endregion

#region Context
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
	opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
#endregion

#region Authentication
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth",configuration =>
{
	configuration.AccessDeniedPath = "/Account/Login";
	configuration.LogoutPath = "/Account/Login";
	configuration.ExpireTimeSpan = TimeSpan.FromHours(1);
	configuration.Cookie.Name = "UserLoginCookie";
});
#endregion

#region UI

builder.Services.AddControllersWithViews();
#endregion

#endregion


#region Middleware
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

#region Localization


app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

#endregion

app.UseRouting();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	var result = context.Set<User>().Any(p => p.Email == "y225012058@sakarya.edu.tr");
	if (!result)
	{
		User user = new()
		{
			FirstName = "H�seyin Serhan",
			LastName = "Kunt",
			Email = "y225012058@sakarya.edu.tr",
			Password = "sau"
		};

		context.Set<User>().Add(user);

		Role? role = context.Set<Role>().Where(p => p.Name == "Admin").FirstOrDefault();

		if (role is null)
		{
			role = new()
			{
				Name = "Admin"
			};
			context.Set<Role>().Add(role);
		}

		context.Set<UserRole>().Add(new()
		{
			RoleId = role.Id,
			UserId = user.Id
		});
		context.SaveChanges();
	}
}

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();
}

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
#endregion






