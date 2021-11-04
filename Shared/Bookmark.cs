using System;

namespace BlazorApp.Shared
{
    public class Bookmark
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        public string Icon { get; set; }

        public int Order { get; set; }
    }
}
