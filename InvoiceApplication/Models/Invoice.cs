using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceApplication.Models
{
    public class Invoice
    {
        public int invoiceId { get; set; }
        public int customerID { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public decimal totalPrice { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}