﻿namespace E_Commerce.Web.Models
{
    public class ProductModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? CategotyName { get; set; }
        public string? imageUrl { get; set; }
    }
}