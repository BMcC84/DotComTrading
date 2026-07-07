using DotComTrading.Data;
using DotComTrading.Services;
using DotComTrading.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DotComTradingDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<DotComTradingDBContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddScoped<WebsiteRepository>();
builder.Services.AddScoped<PortfolioRepository>();

builder.Services.AddHttpClient<DotComTrading.Services.CloudflareRadarClient>((sp, httpClient) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var token = config["Cloudflare:ApiToken"];

    httpClient.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
});

builder.Services.AddHttpClient<RDAPDomainAgeService>();

builder.Services.AddSingleton<RDAPDomainAgeService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();

    return new RDAPDomainAgeService(httpClient);
});

builder.Services.AddHttpClient<MozSeoService>();

builder.Services.AddScoped<UpdateService>();
builder.Services.AddHostedService<MarketMovementService>();

builder.Services.AddScoped<CreateWebsiteService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var websiteRepository = scope.ServiceProvider.GetRequiredService<WebsiteRepository>();
    var cloudflareRadar = scope.ServiceProvider.GetRequiredService<CloudflareRadarClient>();
    await websiteRepository.AddWebsitesFromRadarAsync(cloudflareRadar, 40);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } //For Integration Testing