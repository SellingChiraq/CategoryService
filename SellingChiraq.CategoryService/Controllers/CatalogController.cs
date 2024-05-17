using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SellingChiraq.CategoryService.Core.Application.ViewModels;
using SellingChiraq.CategoryService.Core.Domain;
using SellingChiraq.CategoryService.Infrasctructure;
using SellingChiraq.CategoryService.Infrasctructure.Context;
using System.Net;


namespace SellingChiraq.CategoryService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly CatalogSettings _settings;
    private readonly CatalogContext _dbContext;
    public CatalogController(CatalogContext dbContext, IOptionsSnapshot<CatalogSettings> settings)
    {
        this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this._settings = settings.Value;
    }
    [HttpGet]
    [Route("items")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItem>),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var items = await GetItemsByIdAsync(ids);

            if (!items.Any())
            {
                return BadRequest("ids value invalid");
            }
            return Ok(items);
        }

        var totalItems = await _dbContext.CatalogItems.LongCountAsync();

        var itemsOnPage = await _dbContext.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize * pageIndex)
            .ToListAsync();

        itemsOnPage =  ChangeUriPalceholder(itemsOnPage);

        var model= new PaginatedItemsViewModel<CatalogItem>(pageIndex,pageSize,totalItems,itemsOnPage);

        return Ok(model);
    }

    [HttpGet]
    [Route("items/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CatalogItem),(int)HttpStatusCode.OK)]
    public async Task<ActionResult<CatalogItem>> ItemByIdAsync(int id)
    {
        if (id < 0) return BadRequest();

        var item =await _dbContext.CatalogItems.SingleOrDefaultAsync(c => c.Id == id);

        var baseUri = _settings.PicBaseUrl;

        if(item != null)
        {
            item.PictureUri = baseUri+item.PictureFileName;

            return item;
        }

        return NotFound();

    }

    [Route("items")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult> CreateProductAsync([FromBody] CatalogItem product)
    {
        var item = new CatalogItem
        {
            CatalogBrandId = product.CatalogBrandId,
            CatalogTypeId = product.CatalogTypeId,
            Description = product.Description,
            Name = product.Name,
            PictureFileName = product.PictureFileName,
            Price = product.Price
        };

        _dbContext.CatalogItems.Add(item);

        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(ItemByIdAsync), new { id=item.Id},null);
    }

    private async Task<List<CatalogItem>> GetItemsByIdAsync(string ids)
    {
        var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

        if (!numIds.All(nid => nid.Ok))
        {
            return new List<CatalogItem>();
        }

        var idsToSelect=numIds.Select(id=>id.Value);

        var items = await _dbContext.CatalogItems.Where(ci=>idsToSelect.Contains(ci.Id)).ToListAsync();

        items= ChangeUriPalceholder(items);

        return items;
    }



    private List<CatalogItem> ChangeUriPalceholder(List<CatalogItem> items)
    {
        var baseUri = _settings.PicBaseUrl;

        foreach(var item in items)
        {
            if (item != null)
            {
                item.PictureUri= baseUri+item.PictureFileName;
            }
        }

        return items;
    }
}

