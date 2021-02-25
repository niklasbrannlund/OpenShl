using System;

namespace OpenShl.Models
{
    
    public class Article    {
        public string article_id { get; set; } 
        public string article_url { get; set; } 
        public Author author { get; set; } 
        public string publish_date { get; set; } 
        public string team_code { get; set; } 
        public string title { get; set; } 
    }
    
    public class Author    {
        public string email { get; set; } 
        public string name { get; set; } 
        public string title { get; set; } 
    }


}