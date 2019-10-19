using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PurchaseList.Models;

namespace PurchaseList.Controllers
{
    public class BookingsController : Controller
    {
        private TravelExpertsEntities3 db = new TravelExpertsEntities3();
        //Modified by John, Bachir, Mohammed
        // GET: Bookings
        public ActionResult Index()
        {
            //if there is a customer logged in, display all booking package for this customer
            if (Session["CustomerID"] != null)
            {
                int customerId = Convert.ToInt32( Session["CustomerID"]);
                var bookings =(from pl in db.Bookings.Include(b => b.Customer).Include(b => b.Package)
                               where (pl.CustomerId == customerId)
                               orderby pl.BookingDate descending
                               select pl);
                decimal? total = 0;
                //calculate the total charge of the bookings from the customer
                foreach (Booking b in bookings)
                {
                    decimal? price = b.Package.PkgBasePrice*(Convert.ToDecimal(b.TravelerCount));// - b.Package.PkgAgencyCommission;
                    total += price;
                }
                
                ViewBag.PriceSummary = total;

                return View(bookings);
            }
            else   //if there is no customer logged in, go to login page.
            {
                return RedirectToAction("Login", "Customer");
            }
          
        }


        //Packages initial page
        public ActionResult Packages()
        {
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "TripTypeId", "TTName");
            return View();
        }

       // POST: Bookings/Create a package
       //only traveltype and travelcount required. The other info is filled automatically
       //Get CustomerID from login user
       //Get packageID from selected package
       //Get booking date from DateTime now
       //Get BookingNo by random letters and numbers with packageID, booking date and traveler count

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Packages([Bind(Include = "BookingId,BookingDate,BookingNo,CustomerId,PackageId,TravelerCount,TripTypeId")] Booking bk)
        {
            
            if (ModelState.IsValid)    //if all validation correct
            {
                //if a customer is interested in a package and logged in, it will display the package the customer already selected
                if(Session["SelectedPackageID"]!= null)    
                {
                    int selectedPackageId = Convert.ToInt32(Session["SelectedPackageID"]);
                    int customerId = Convert.ToInt32(Session["CustomerID"]);
                    DateTime bookingDate = DateTime.Now;
                    Booking booking = new Booking();
                    booking.PackageId = selectedPackageId;
                    booking.CustomerId = customerId;
                    booking.BookingDate = bookingDate;
                    booking.BookingNo = RandomBookingNo().ToUpper()+ selectedPackageId + bookingDate.Day + bk.TravelerCount;
                    booking.TravelerCount = bk.TravelerCount;
                    booking.TripTypeId = bk.TripTypeId;

                    Package thisPackage = db.Packages.Find(selectedPackageId);
                    Customer thisCustomer = db.Customers.Find(customerId);

                    string summary = ((decimal)(booking.TravelerCount) * (thisPackage.PkgBasePrice)).ToString("C");

                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    ModelState.Clear();
                    ViewBag.SuccessMessage = "Booking Successful, Thank you for your purchase.";
                    //After booking successfully, the page jump to the booking history of the customer
                    string toEmail = thisCustomer.CustEmail;
                    string subject = "Booking Confirmed";
                    string emailBody = $"<p>Hi {thisCustomer.CustFirstName},<br/>Thank you for your booking with Travel Experts.<br/>" +
                            $"Your booking is: {thisPackage.PkgName}, start on {thisPackage.PkgStartDate} to {thisPackage.PkgEndDate}" +
                            $" The travel count is {booking.TravelerCount}, the total charge is{summary} <br/> <br/>Travel Experts</p>";
                    var customercontroller = DependencyResolver.Current.GetService<CustomersController>();
                    var sendConfirmBookingEmail = customercontroller.SendEmail(toEmail, subject, emailBody);
                    //string successMessage = "Booking Successful, Thank you for your purchase.";
                    //ClientScript.RegisterOnSubmitStatement(this.GetType(), "ConfirmSubmit", successMessage);
                    //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('An booking email is sent to you.')</SCRIPT>");
                    return RedirectToAction("Index", "Bookings");
                }
                else
                {
                    //if not booking correctly, the page return to the package list page.
                    return RedirectToAction("Index", "Packages");
                }
               
            }
            //use DropDownList to select TripTypeId. display TTName, get TripTypeId.
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "TripTypeId", "TTName", bk.TripTypeId);
            return View(bk);
        }

        //create a string with random letters and numbers to used for create BookingNo.
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public string RandomBookingNo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
