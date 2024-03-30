using System.Collections.Generic;
using ShoppingCart.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShoppingCart.Api.Uow;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingCart.Api.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public ProductController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] Dictionary<string, string> queryParams)
        {
            var result = await _uow.SearchRepository.Search(queryParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var product = await _uow.ProductRepository.GetProduct(id, userId);
            if (product == null)
                return BadRequest("Product not found!");
            return Ok(product);
        }
    }
}
