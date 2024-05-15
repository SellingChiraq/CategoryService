using System.Data.SqlClient;
using System.IO.Compression;
using Polly;

using SellingChiraq.CategoryService.Core.Domain;


namespace SellingChiraq.CategoryService.Infrasctructure.Context;
public class CatalogContextSeed
{
    public async Task SeedaAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
    {
        var policy = Policy.Handle<SqlException>().
            WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timesSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "Exception with message detected on attempt");
                }
            );

        var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
        var picturePath = "Pics";

        await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath, logger));
    }

    private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath, ILogger logger)
    {
        if (!context.CatalogBrands.Any())
        {
            await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));

            await context.SaveChangesAsync();
        }

        if(!context.CatalogTypes.Any())
        {
            await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));

            await context.SaveChangesAsync();
        }


        if (!context.CatalogItems.Any())
        {
            await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath,context));

            await context.SaveChangesAsync();

            GetCatalogItemPictures(setupDirPath, picturePath);
        }
       
    }

    private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentPath)
    {
        IEnumerable<CatalogType> GetPreconfiguredTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType{ Id=2,Type="Sweeter"}
            };

        }
        string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredTypes();
        }

        var fileContent = File.ReadAllLines(fileName);

        var list = fileContent.Select(x => new CatalogType { Type = x.Trim('"') }).Where(i => i != null);

        return list ?? GetPreconfiguredTypes();

    }

    private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentPath)
    {
        IEnumerable<CatalogBrand> GetPreconfiguredBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand{ Id=2,Brand="Sweeter"}
            };

        }
        string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredBrands();
        }

        var fileContent = File.ReadAllLines(fileName);

        var list = fileContent.Select(x => new CatalogBrand { Brand = x.Trim('"') }).Where(i => i != null);

        return list ?? GetPreconfiguredBrands();

    }

    private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
    {
        IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
               new CatalogItem{CatalogTypeId=2,CatalogBrandId=2,Description=".Net Black Hoodie",Name="Hoodie"}
            };
        }

        string fileName = Path.Combine(contentPath, "CatalogItems.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredItems();
        }

        var catalogTyepIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
        var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);

        var fileContent = File.ReadAllLines(fileName)
            .Skip(1)
            .Select(i => i.Split(','))
            .Select(i => new CatalogItem()
            {
                CatalogTypeId = catalogTyepIdLookup[i[0]],
                CatalogBrandId = catalogBrandIdLookup[i[1]],
                Description = i[2].Trim('"').Trim(),
                Name = i[3].Trim('"').Trim()
            });

        return fileContent;
    }
    private void GetCatalogItemPictures(string contentPath, string picturePath)
    {
        picturePath ??= "pics";

        if (picturePath != null)
        {
            DirectoryInfo directory = new DirectoryInfo(picturePath);

            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                fileInfo.Delete();
            }

            string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
            ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
        }
    }
}
