using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Wenxuecity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService _service;
        private readonly GlobalSettings _settings;

        public HomeController(IService service, IOptions<GlobalSettings> settings)
        {
            _service = service;
            _settings = settings.Value;
        }

        public async Task<IActionResult> Index()
        {
            var html = await _service.GetPageAsync(_settings.BaseUrl);

            return View(_service.GetArticleList(html));
        }

        public IActionResult Error() => View();
    }
}
