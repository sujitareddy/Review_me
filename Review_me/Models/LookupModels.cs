using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Review_me.Models
{
    public class Rootobject
    {
        public string status { get; set; }
        public string copyright { get; set; }
        public int num_results { get; set; }
        public DateTime last_modified { get; set; }
    }

    public class CategoryList : Rootobject
    {
        public Category[] results { get; set; }

    }

    public class Category
    {
        public int category_id { get; set; }
        [Required]
        public string list_name { get; set; }
        public string display_name { get; set; }
        public string list_name_encoded { get; set; }
        public string oldest_published_date { get; set; }
        public string newest_published_date { get; set; }
        public string updated { get; set; }
    }


    public class WeeklyHitsObject : Rootobject
    {
        public WeeklyHits results { get; set; }
    }

    public class WeeklyHits
    {
        public int weeklyhitsId { get; set; }
        public string list_name { get; set; }
        public string list_name_encoded { get; set; }
        public string bestsellers_date { get; set; }
        public string published_date { get; set; }
        public string published_date_description { get; set; }
        public string next_published_date { get; set; }
        public string previous_published_date { get; set; }
        public string display_name { get; set; }
        public int normal_list_ends_at { get; set; }
        public string updated { get; set; }
        public Book[] books { get; set; }
    }


}
