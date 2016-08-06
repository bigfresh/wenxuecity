using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wenxuecity
{
    public interface IService
    {
        IEnumerable<Link> GetArticleList(string html);

        Task<string> GetPageAsync(string url);

        string GetArticleContent(string html);
    }
}