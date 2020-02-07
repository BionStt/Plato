﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("navigation")]
    public class NavigationTagHelper : TagHelper
    {

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("name")]
        public string Name { get; set; } = "Site";

        [HtmlAttributeName("collaspsable")]
        public bool Collaspsable { get; set; }
        
        [HtmlAttributeName("collapse-css")]
        public string CollapseCss { get; set; } = "collapse";

        [HtmlAttributeName("model")]
        public object Model { get; set; }
        
        [HtmlAttributeName("link-css-class")]
        public string LinkCssClass { get; set; } = "nav-link";

        [HtmlAttributeName("li-css-class")]
        public string LiCssClass { get; set; } = "nav-item";

        [HtmlAttributeName("enable-list")]
        public bool EnableList { get; set; } = true;

        [HtmlAttributeName("child-ul-css-class")]
        public string ChildUlCssClass { get; set; } = "nav flex-column";

        [HtmlAttributeName("child-ul-inner-css-class")]
        public string ChildUlInnerCssClass { get; set; }

        [HtmlAttributeName("selected-css-class")]
        public string SelectedCssClass { get; set; } = "active";

        private static readonly string NewLine = Environment.NewLine;
        private int _level;
        private int _index;
        private object _cssClasses;

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly INavigationManager _navigationManager;        
        private readonly IViewHelperFactory _viewHelperFactory;
        private IViewDisplayHelper _viewDisplayHelper;

        public NavigationTagHelper(
            IActionContextAccessor actionContextAccessor,
            INavigationManager navigationManager,            
            IViewHelperFactory viewHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _navigationManager = navigationManager;            
            _viewHelperFactory = viewHelperFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Get default CSS classes
            _cssClasses = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value;

            // Ensure no surrounding element
            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;

            // Get action context
            var actionContext = _actionContextAccessor.ActionContext;

            // Add navigation model if provided to action context for
            // optional use within any navigation builders later on
            if (Model != null)
            {
                actionContext.HttpContext.Items[Model.GetType()] = Model;
            }

            // Build navigation
            var sb = new StringBuilder();
            var items = _navigationManager.BuildMenu(Name, actionContext);
            var itemsList = items.ToList();
            if (itemsList.Any())
            {
                await BuildNavigationRecursivelyAsync(itemsList, sb);
                output.PreContent.SetHtmlContent(sb.ToString());
            }
            else
            {
                output.SuppressOutput();
            }

        }

        bool IsLinkExpandedOrChildSelected(MenuItem menuItem)
        {
            var output = IsChildSelected(menuItem);
            if (output)
            {
                return true;
            }

            output = IsCollapseCssVisible();
            if (output)
            {
                return true;
            }

            return false;
        }

        bool IsChildSelected(List<MenuItem> items)
        {
            foreach (var item in items)
            {
                if (item.Selected) return true;
                if (item.Items.Count > 0) IsChildSelected(item.Items);
            }
            return false;
        }

        bool IsChildSelected(MenuItem menuItem)
        {
            foreach (var item in menuItem.Items)
            {
                if (item.Selected) return true;
                if (item.Items.Count > 0) IsChildSelected(item);
            }
            return false;
        }

        bool IsCollapseCssVisible()
        {
            if (CollapseCss.IndexOf("show", StringComparison.Ordinal) >= 0)
            {
                return true;
            }
            return false;
        }

        async Task<string> BuildNavigationRecursivelyAsync(
            List<MenuItem> items, 
            StringBuilder sb)
        {
            
            // reset index
            if (_level == 0)
                _index = 0;
            
            sb.Append(NewLine);
            AddTabs(_level, sb);
            
            if (_level > 0 && Collaspsable)
            {
                var collapseCss = IsChildSelected(items) ? CollapseCss + " show" : CollapseCss;
                sb.Append("<div class=\"")
                    .Append(collapseCss)
                    .Append("\" id=\"")
                    .Append("menu-")
                    .Append(_level > 0 ? (_index - 1).ToString() : "root")
                    .Append("\">");
            }

            if (EnableList)
            {
                var ulClass = _cssClasses;
                if (_level > 0)
                {
                    ulClass = ChildUlCssClass;
                }
               
                // ul
                sb.Append("<div class=\"")
                    .Append(ulClass)
                    .Append("\">")
                    .Append(NewLine);

                if (!string.IsNullOrEmpty(ChildUlInnerCssClass) && _level > 0)
                {
                    sb.Append("<div class=\"")
                        .Append(ChildUlInnerCssClass)
                        .Append("\">")
                        .Append(NewLine);
                }


            }

            var index = 0;
            foreach (var item in items)
            {
                
                AddTabs(_level + 1, sb);

                if (EnableList)
                {
                    sb.Append("<div class=\"")
                        .Append(GetListItemClass(items, item, index))
                        .Append("\">");
                }

                try
                {
                    sb.Append(item.View != null
                        ? await BuildViewAsync(item)
                        : BuildLink(item));
                }
                catch (Exception e)
                {
                    sb.Append("<p class=\"text-danger font-weight-bold\">");
                    if (item.View != null)
                    {
                        sb
                            .Append("An exception occurred whilst invoking the view \"")
                            .Append(item.View.ViewName)
                            .Append("\" for the \"")
                            .Append(Name)
                            .Append("\" navigation tag helper, Exception message: ")
                            .Append(e.Message);

                    }
                    else
                    {
                        sb
                            .Append(
                                "An exception occurred whilst building a link for the navigation tag helper with the name \"")
                            .Append(Name)
                            .Append("\", Exception message: ")
                            .Append(e.Message);
                    }

                    sb.Append("</p");

                }

                _index++;

                if (item.Items.Count > 0)
                {
                    _level++;
                    await BuildNavigationRecursivelyAsync(item.Items, sb);
                    AddTabs(_level, sb);
                    _level--;
                }

                if (EnableList)
                {
                    sb.Append("</div>").Append(NewLine);
                }

                index += 1;
            }

            AddTabs(_level, sb);
            if (EnableList)
            {

                if (!string.IsNullOrEmpty(ChildUlInnerCssClass) && _level > 0)
                {
                    sb.Append("</div>")
                        .Append(NewLine);
                }

                sb.Append("</div>")
                    .Append(NewLine);
                
            }

            if (_level > 0 && Collaspsable)
            {
                sb.Append("</div>");
            }

            return sb.ToString();

        }

        string BuildLink(MenuItem item)
        {
            var linkClass = _level == 0 | Collaspsable
                ? LinkCssClass
                : "dropdown-item";

            if (item.Items.Count > 0)
            {
                if (!Collaspsable)
                {
                    if (!string.IsNullOrEmpty(linkClass))
                        linkClass += " ";
                    linkClass += "dropdown-toggle";
                }
            }

            foreach (var className in item.Classes)
            {
                if (!string.IsNullOrEmpty(linkClass))
                    linkClass += " ";
                linkClass += className;
            }

            if (item.Selected)
            {
                if (!string.IsNullOrEmpty(SelectedCssClass))
                {
                    linkClass += " " + SelectedCssClass;
                }

            }

            var targetEvent = "";
            var targetCss = " data-toggle=\"dropdown\"";
            if (Collaspsable)
            {
                targetCss = " data-toggle=\"collapse\"";
                targetEvent = $" data-target=\"#menu-{_index}\" aria-controls=\"#menu-{_index}\"";
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(item.DividerCss))
            {
                sb.Append("<div class=\"")
                    .Append(item.DividerCss)
                    .Append("\"></div>");
            }
            else
            {

                sb.Append("<a class=\"")
                    .Append(linkClass)
                    .Append("\" href=\"")
                    .Append(item.Href)
                    .Append("\"")
                    .Append(item.Items.Count > 0 ? targetEvent : "")
                    .Append(item.Items.Count > 0 ? targetCss : "")
                    .Append(" aria-expanded=\"")
                    .Append(IsLinkExpandedOrChildSelected(item).ToString().ToLower())
                    .Append("\"");

                if (item.Attributes.Count > 0)
                {
                    var i = 0;
                    foreach (var attr in item.Attributes)
                    {
                        if (i == 0)
                        {
                            sb.Append(" ");
                        }

                        sb.Append(attr.Key)
                            .Append("=\"")
                            .Append(attr.Value.ToString())
                            .Append("\"");
                        if (i < item.Attributes.Count)
                        {
                            sb.Append(" ");
                        }
                    }
                }

                sb.Append(">");

                if (!String.IsNullOrEmpty(item.IconCss))
                {
                    sb.Append("<i class=\"")
                        .Append(item.IconCss)
                        .Append("\"></i>");
                }

                sb.Append("<span class=\"nav-text\">")
                    .Append(item.Text.Value)
                    .Append("</span>");

                if (!String.IsNullOrEmpty(item.BadgeText))
                {
                    sb.Append("<span class=\"")
                        .Append(item.BadgeCss)
                        .Append("\">")
                        .Append(item.BadgeText)
                        .Append("</span>");
                }

                sb.Append("</a>");

            }

            return sb.ToString();
        }

        async Task<string> BuildViewAsync(MenuItem item)
        {

            EnsureViewHelper();            
            var viewResult = await _viewDisplayHelper.DisplayAsync(new View(item.View.ViewName, item.View.Model));
            return viewResult.Stringify();

        }

        void EnsureViewHelper()
        {
            if (_viewDisplayHelper == null)
            {
                _viewDisplayHelper = _viewHelperFactory.CreateHelper(ViewContext);
            }
        }

        string GetListItemClass(
            ICollection items,
            MenuItem item,
            int index)
        {

            var sb = new StringBuilder();
            sb.Append(LiCssClass);

            if (item.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                if (!Collaspsable)
                {
                    sb.Append("dropdown");
                }
                
            }

            if (_level > 0)
            {
                if (!Collaspsable)
                {
                    if (!string.IsNullOrEmpty(sb.ToString()))
                        sb.Append(" ");
                    sb.Append("dropdown-submenu");
                }
            }

            if (index == 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("first");
            }

            if (index == items.Count - 1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("last");
            }

            return sb.ToString();

        }

        StringBuilder AddTabs(int level, StringBuilder sb)
        {
            for (var i = 0; i < level; i++)
            {
                sb
                    .Append("   ");
            }

            return sb;

        }

    }

}
