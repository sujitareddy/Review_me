using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Review_me.Models
{
    public class Book
    {
        [Key]
        public int bookId { get; set; }
        public int rank { get; set; }
        public int rank_last_week { get; set; }
        public int weeks_on_list { get; set; }
        public int asterisk { get; set; }
        public int dagger { get; set; }
        public string primary_isbn10 { get; set; }
        public string primary_isbn13 { get; set; }
        public string publisher { get; set; }
        public string description { get; set; }
        public string price { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string contributor { get; set; }
        public string contributor_note { get; set; }
        public string book_image { get; set; }
        public int book_image_width { get; set; }
        public int book_image_height { get; set; }
        public string amazon_product_url { get; set; }
        public string age_group { get; set; }
        public string book_review_link { get; set; }
        public string first_chapter_link { get; set; }
        public string sunday_review_link { get; set; }
        public string article_chapter_link { get; set; }
        [JsonProperty("isbns"), NotMapped]
        public Isbn[] isbns_array { get; set; }
        
        [NotMapped]
        public BuyLink[] buy_links { get; set; }
        public string book_uri { get; set; }
        public List<BuyLink> buyLinks { get; set; }
        [JsonProperty("isbns_List")]
        public List<Isbn> isbns { get; set; }
        public List<ReviewLink> reviewLinks { get; set; }
    }


    public class BuyLink
    {
        [Key]
        public int buyLinkId { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int? bookId { get; set; }
        public Book book { get; set; }
    }

    public class Isbn
    {
        [Key]
        public int isbnId { get; set; }
        public string isbn10 { get; set; }
        public string isbn13 { get; set; }
        public int? bookId { get; set; }
        public Book book { get; set; }
    }

    public class ReviewLink
    {
        [Key]
        public int reviewLinkId { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int? bookId { get; set; }
        public Book book { get; set; }
    }
}
