using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceApplication.Models
{
    public class Customer
    {
        public int customerId { get; set; }
        public string customerName { get; set; }
        public string customerAddress { get; set; }
        public string city { get; set; }
    }
}