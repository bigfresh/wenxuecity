using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Wenxuecity.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IService _service;

        public ArticleController(IService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string url)
        {
            var html = await _service.GetPageAsync(url);

            ViewBag.HtmlStr = _service.GetArticleContent(html);

            return View();
        }
    }
}
