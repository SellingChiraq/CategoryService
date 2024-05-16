using SellingChiraq.CategoryService.Extensions;
using SellingChiraq.CategoryService.Infrasctructure;
using SellingChiraq.CategoryService.Infrasctructure.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection("CatalogSettings"));
builder.Services.ConfigureDbContext(builder.Configuration);

var app = builder.Build();

app.MigrateDbContext<CatalogContext>((context, services) =>
{
    var env = services.GetService<IWebHostEnvironment>();

    var logger = services.GetService<ILogger<CatalogContextSeed>>();

    new CatalogContextSeed().SeedAsync(context, env, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

