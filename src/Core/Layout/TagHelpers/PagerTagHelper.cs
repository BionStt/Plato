﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Navigation.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {

        private const int NumberOfPagesToShow = 4;
        private int _totalPageCount;
        
        public PagerOptions Model { get; set; }

        public IStringLocalizer T { get; set; }

        public string FirstText { get; set; }

        public string PreviousText { get; set; }

        public string NextText { get; set; }

        public string LastText { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }        
  
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IUrlHelper _urlHelper;
        private string pageKey = "pager.page";

        private RouteValueDictionary _routeData;

        public PagerTagHelper(
            IStringLocalizer<PagerTagHelper> localizer,
            IActionContextAccessor actionContextAccesor,
            IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccesor = actionContextAccesor;
            _urlHelper = urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
            T = localizer;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Calculate total pages
            _totalPageCount = Model.Size > 0 ? (int)Math.Ceiling((double)Model.Total / Model.Size) : 1;

            // Get route data
            _routeData = new RouteValueDictionary(_actionContextAccesor.ActionContext.RouteData.Values);

            // Define taghelper
            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");
            
            output.TagMode = TagMode.StartTagAndEndTag;

            // No need to display if we only have a single page
            if (_totalPageCount > 1)
            {
                output.Content.SetHtmlContent(await Build());
            }

        }

        async Task<IHtmlContent> Build()
        {

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<ul class=\"pagination\">")
                .AppendHtml(BuildFirst())
                .AppendHtml(BuildPrevious())
                .AppendHtml(BuildLinks())
                .AppendHtml(BuildNext())
                .AppendHtml(BuildLast())
                .AppendHtml(BuildLabel())
                .AppendHtml("</ul>");

            return await Task.FromResult(htmlContentBuilder);

        }

        HtmlString BuildFirst()
        {

            if (Model.Page <= 2)
            {
                return new HtmlString(string.Empty);
            }

            _routeData.Remove(pageKey);
            var url = _urlHelper.RouteUrl(new UrlRouteContext { Values = _routeData });

            var text = FirstText ?? T["&laquo;"];

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"")
                .AppendHtml(url)
                .AppendHtml("\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();

        }

        HtmlString BuildPrevious()
        {

            if (Model.Page == 1)
            {
                return new HtmlString(string.Empty);
            }

            var text = PreviousText ?? T["Prev"];

            _routeData[pageKey] = Model.Page - 1;
            var url = _urlHelper.RouteUrl(new UrlRouteContext { Values = _routeData });
            
            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"")
                .AppendHtml(url)
                .AppendHtml("\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();
        }

        HtmlString BuildLinks()
        {

            if (Model == null)
            {
                throw new ArgumentNullException(nameof(Model));
            }

            if (Model.Size == 0)
            {
                throw new ArgumentNullException(nameof(Model.Size));
            }
               
            var currentPage = Model.Page;
            if (currentPage < 1)
                currentPage = 1;
            
            var firstPage = Math.Max(1, Model.Page - (NumberOfPagesToShow / 2));
            var lastPage = Math.Min(_totalPageCount, Model.Page + (int) (NumberOfPagesToShow / 2));
            
            IHtmlContentBuilder output = null;

            if (NumberOfPagesToShow > 0 && lastPage > 1)
            {

                output = new HtmlContentBuilder();

                for (var i = firstPage; i <= lastPage; i++)
                {

                    if (i == 1)
                        _routeData.Remove(pageKey);
                    else
                        _routeData[pageKey] = i;

                    var url = _urlHelper.RouteUrl(new UrlRouteContext {Values = _routeData});

                    var builder = new HtmlContentBuilder();
                    output.AppendHtml(builder
                        .AppendHtml("<li class=\"page-item")
                        .AppendHtml(i == Model.Page ? " active" : "")
                        .AppendHtml("\">")
                        .AppendHtml("<a class=\"page-link\" href=\"")
                        .AppendHtml(url)
                        .AppendHtml("\" aria-label=\"")
                        .AppendHtml(i.ToString())
                        .AppendHtml("\">")
                        .AppendHtml(i.ToString())
                        .AppendHtml("</a>")
                        .AppendHtml("</li>"));
                }
            }

            if (output != null)
            {
                return output.ToHtmlString();
            }

            return HtmlString.Empty;

        }

        HtmlString BuildNext()
        {

            if (Model.Page == _totalPageCount)
            {
                return new HtmlString(string.Empty);
            }

            var text = NextText ?? T["Next"];

            _routeData[pageKey] = Model.Page + 1;
            var url = _urlHelper.RouteUrl(new UrlRouteContext { Values = _routeData });
            
            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"")
                .AppendHtml(url)
                .AppendHtml("\" aria-label=\"Next\">")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();

        }

        HtmlString BuildLast()
        {

            if (Model.Page >= _totalPageCount - 1)
            {
                return new HtmlString(string.Empty);
            }

            var text = LastText ?? T["&raquo;"];

            _routeData[pageKey] = _totalPageCount;
            var url = _urlHelper.RouteUrl(new UrlRouteContext { Values = _routeData });
            
            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"")
                .AppendHtml(url)
                .AppendHtml("\" aria-label=\"Last\">")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();
        }


        HtmlString BuildLabel()
        {

            var page = T["Page"];
            var of = T["of"];
            var results = T["Results"];

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<div class=\"p-2 text-muted\">")
                .Append(page)
                .Append(" ")
                .Append(Model.Page.ToString())
                .Append(" ")
                .Append(of)
                .Append(" ")
                .Append(_totalPageCount.ToString())
                .Append(", ")
                .Append(Model.Total.ToString())
                .Append(" ")
                .Append(results)
                .AppendHtml("</div>")
                .AppendHtml("</li>");;

            return htmlContentBuilder.ToHtmlString();
        }

    }

}
