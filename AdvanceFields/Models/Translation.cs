using Microsoft.AspNetCore.Mvc;

namespace AdvanceFields.Models
{
    public class Translation
    {
        public Success success;
        public Content contents;
    }

    public class Content
    {
        public string translated { get; set; }
        public string text { get; set; }
        public string translation { get; set; }
    }

    public class Success
    {
        public int total { get; set; }
    }
}
