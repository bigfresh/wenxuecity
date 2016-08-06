﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Wenxuecity
{
    public class Service : IService
    {
        private readonly GlobalSettings _settings;

        public Service(IOptions<GlobalSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> GetPageAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(url))
                using (var content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public IEnumerable<Link> GetArticleList(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var list = new List<Link>();

            var test = doc.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("col"));


            list.AddRange(GetLinks(doc.DocumentNode.SelectNodes(@"/html/body/div[4]/div[2]/div[2]")));
            list.AddRange(GetLinks(doc.DocumentNode.SelectNodes(@"/html/body/div[4]/div[2]/div[3]")));

            return list;
        }

        public string GetArticleContent(string html)
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var articleContent = doc.DocumentNode.SelectSingleNode("//*[@id=\"articleContent\"]");

            var images = articleContent.Descendants("img").GetEnumerator();

            while (images.MoveNext())
            {
                var img = images.Current.Attributes.SingleOrDefault(a => a.Name == "src");

                if (img != null && IsInternalImgLink(img.Value))
                {
                    images.Current.SetAttributeValue("src",
                        $"{_settings.BaseUrl}{images.Current.Attributes.Single(a => a.Name == "src").Value}");
                }

            }

            return articleContent.InnerHtml;
        }

        private bool IsInternalImgLink(string href) => _settings.InternalImgUrls.Any(href.Contains);

        private IEnumerable<Link> GetLinks(HtmlNodeCollection nodes)
        {
            var list = new List<Link>();

            var links = nodes.Descendants("a").GetEnumerator();

            while (links.MoveNext())
            {
                var l = links.Current.Attributes.SingleOrDefault(a => a.Name == "href" && a.Value.Contains("news"));

                if (l != null)
                {
                    list.Add(new Link
                    {
                        Name = links.Current.InnerHtml,
                        Url = $"{_settings.BaseUrl}{l.Value}"
                    });
                }
            }

            return list;
        }
    }
}
