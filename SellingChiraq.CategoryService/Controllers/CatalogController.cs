using Microsoft.AspNetCore.Mvc;
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
public class CatalogController:ControllerBase
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
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>),(int)HttpStatusCode.OK)]
    public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10,[FromQuery] int pageIndex = 0,string id = null)
    {
        return Ok();
    }
}

