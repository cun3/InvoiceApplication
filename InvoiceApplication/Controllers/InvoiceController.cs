using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InvoiceApplication.Models;
using PagedList;


namespace InvoiceApplication.Controllers
{
    public class InvoiceController : Controller
    {
        private CustomerContext db = new CustomerContext();
       

        // GET: Invoice
        public ActionResult Index(int? page)
            {
                
            //Session Management
                if (Session["MaxLimit"] == null && Session["Warning"] == null)
                {
                    ViewData["MaxLimit"] = 1000;
                    ViewData["Warning"] = 850;
                    Session["MaxLimit"] = 1000;
                    Session["Warning"] = 850;
                }
                else
                {
                    if (Session["MaxLimit"] == null)
                    {
                        ViewData["MaxLimit"] = 1000;
                    }
                    else if (Session["Warning"] == null)
                    {
                        ViewData["Warning"] = 850;
                    }
                    ViewData["MaxLimit"] = (int)(Session["MaxLimit"]);
                    ViewData["Warning"] = (int)(Session["Warning"]);
                }
                //var invoices = db.Invoices.Include(i => i.Customer).Include(i => i.Product);
                var invoices = from inv in db.Invoices  
                               select inv;
                invoices=invoices.OrderBy(s => s.invoiceId);

                int pageSize = 8;
                int pageNumber = (page ?? 1);

                return View(invoices.ToPagedList(pageNumber, pageSize));
        }

        
        // GET: Invoice/Create
        public ActionResult Create()
        {
            
            ViewBag.customerID = new SelectList(db.Customers, "customerId", "customerName");
            ViewBag.productId = new SelectList(db.Products, "productId", "productName");
            return View();
        }

        // POST: Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "invoiceId,customerID,productId,quantity,totalPrice")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var price = (from prod in db.Products
                                          where prod.productId == invoice.productId
                                          select prod.Price);
                decimal price1 = price.First();
                invoice.totalPrice = invoice.quantity * price1;
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.customerID = new SelectList(db.Customers, "customerId", "customerName", invoice.customerID);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", invoice.productId);
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.customerID = new SelectList(db.Customers, "customerId", "customerName", invoice.customerID);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", invoice.productId);
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "invoiceId,customerID,productId,quantity,totalPrice")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var price = (from prod in db.Products
                             where prod.productId == invoice.productId
                             select prod.Price);
                decimal price1 = price.First();
                invoice.totalPrice = invoice.quantity * price1;
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.customerID = new SelectList(db.Customers, "customerId", "customerName", invoice.customerID);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", invoice.productId);
            return View(invoice);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //POST Invoice/Validate
        //Used to validate the total price limit.
        [HttpPost]
        public ActionResult Validate(int quantity, int pid)
        {
            int maxLimit;
            int warning;
            decimal price1;
            decimal total;
            decimal totalprice;

            var price = (from prod in db.Products
                         where prod.productId == pid
                         select prod.Price);
            price1 = price.First();


            total = db.Invoices.Sum(x => x.totalPrice);


            totalprice = total + (quantity * price1);

            if (Session["MaxLimit"] == null)
            {
                 maxLimit = 1000;
            }
            else if (Session["Warning"] == null)
            {
                 warning = 850;
            }

            maxLimit = (int)(Session["MaxLimit"]);
            warning = (int)(Session["Warning"]);

            if (totalprice > maxLimit) 
            {
                return Content("Limit Exceeded.");
            }
            else if (totalprice >= warning)
            {
                return Content("Warning.");
            }
            else 
            {
                return Content("Limit Not Exceeded.");
            }
        }

        [HttpPost]
        public ActionResult StoreMaxLimitData(int MaxLimit)
        {
           Session["MaxLimit"] = MaxLimit;
           return Content("Success");
        }

        [HttpPost]
        public ActionResult StoreWarningLimit(int Warning)
        {
            Session["Warning"] = Warning;
            return Content("Success");
        }

    }
}
