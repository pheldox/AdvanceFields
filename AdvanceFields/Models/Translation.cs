using Microsoft.AspNetCore.Mvc;
using System;

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

    public class RqTranslate
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Translated { get; set; }
        public DateTime CreatedWhen { get; set; }
    }

    
}
