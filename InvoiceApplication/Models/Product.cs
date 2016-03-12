using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceApplication.Models
{
    public class Product
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public decimal Price { get; set; }
    }
}