using System;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {

        public string Title { get; set; }

        public string Author { get; set; }

        [Column(ColumnType.Label)]
        public decimal Price { get; set; }

        public string Year { get; set; }

        [Column(ColumnType.ComboBox)]
        public BookBinding Binding { get; set; }

        [Display(Name = "In Stock")]
        [Column(ColumnType.CheckBox)]
        public bool InStock { get; set; }

        [Column(ColumnType.Button)]
        public string Description { get; set; }

    }
}
