﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Assets.Abstractions
{

    public class Asset
    {

        public string Url { get; set; }

        public string CdnUrl { get; set; }

        public AssetType Type { get; set; }

        public AssetSection Section { get; set; }

        public int Order { get; set; }

        public IHtmlContent InlineContent { get; set; }

        public IDictionary<string, string> Attributes { get; set; }

        public AssetConstraints Constraints { get; set; }             

    }

    public class AssetConstraints
    {

        public string Layout { get; set; }

        public List<AssetConstraint> Routes { get; set; }
    }

    public class AssetConstraint : Dictionary<string, string>
    {
    }

    public enum AssetSection
    {
        Meta,
        Header,
        Body,
        Footer
    }

    public enum AssetType
    {
        IncludeJavaScript,
        IncludeCss,
        Meta
    }

}
