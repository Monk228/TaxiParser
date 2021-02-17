using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxiParser.Services
{
   public class Helpers
    {
        private readonly IBrowsingContext Context;
        private readonly IConfiguration Config;

       
        public Helpers(IBrowsingContext _context, IConfiguration _config)
        {
            Context = _context;
            Config = _config;
        }
        public bool NotNullAndEmpty(string text)
        {
            if (text.Length == 0)
            {
                return false;
            }
            if (text == null)
            {
                return false;
            }
            if (text == "")
            {
                return false;
            }

            return true;
        }
        public IHtmlDocument GetDocument(string html)
        {
            var parser = Context.GetService<IHtmlParser>();

            return parser.ParseDocument(html);
        }

        public List<IElement> TakeElements(string html, Func<IElement, bool> predicate)
        {
            return GetDocument(html).All.Where(predicate).ToList();
        }
    }
}
