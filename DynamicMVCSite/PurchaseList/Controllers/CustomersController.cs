//Modified by Kingsley, John

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PurchaseList.Helpers;
using PurchaseList.Models;

namespace PurchaseList.Controllers
{

    public class CustomersController : Controller
    {
        
        private TravelExpertsEntities3 db = new TravelExpertsEntities3();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Registration()
        {
            return View();
        }
        //Ask for confirm email and confirm password when register
        //all validations are in the Customer.cs
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Registration(Customer customer)
        {
            try
            {

                using (var context = new TravelExpertsEntities3())
                {
                    var chkUser = (from s in context.Customers where s.CustUsername == customer.CustUsername select s).FirstOrDefault();
                    string name = customer.CustFirstName;   //get the customer first name from the customer object 
                    string username = customer.CustUsername;  //get the customer username from the customer object
                    string userPassword = customer.CustPassword; //get the customer password from the customer object
                    if (chkUser == null)
                    {
                        
                        var password = Helper.HashEncrypt(customer.CustPassword);

                        customer.CustPassword = password;
                        customer.ConfirmPassword = password;

                        context.Customers.Add(customer);
                        context.SaveChanges();
                        SendEmail(customer.CustEmail, "Registration Confirmed",
                            $"<p>Hi {name},<br/>Thank you for registering with Travel Experts where you explore, journey, discover and adventure.<br/>" +
                            $"Your username: {username}<br/> Your password: {userPassword}<br/> <br/> Travel Experts</p>");

                        ModelState.Clear();
                        ModelState.Clear();
                        ViewBag.SuccessMessage = "Registration Successful";
                        //return RedirectToAction("LogIn", "Login");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Username Already Exists! Please enter a new username.";
                    }

                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        //If the user did not login
        //ask for login and save the user's CustomerID and name to Session

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LogIn(string userName, string password)
        {
            if (Session["CustomerID"] == null)
            {
                try
                {
                    using (var context = new TravelExpertsEntities3())
                    {
                        var getUser = (from s in context.Customers where s.CustUsername == userName select s).FirstOrDefault();
                        if (getUser != null)
                        {
                            //var hashCode = Helper.GeneratePassword(10);   //get the salt from the database
                            //Password Hasing Process Call Helper Class Method    
                            // var encodingPasswordString = Helper.EncodePassword(password, hashCode);  //has the input password again the salt stored in the database
                            var encodingPasswordString = Helper.HashEncrypt(password);  //encrypt pass word before checking database
                            Session["CustomerID"] = getUser.CustomerId.ToString();
                            Session["CustomerName"] = getUser.CustFirstName;

                            //Check Login Detail User Name Or Password    
                            var query = (from s in context.Customers where (s.CustUsername == userName) && s.CustPassword.Equals(encodingPasswordString) select s).FirstOrDefault();
                            if (query != null)
                            {
                               
                                //RedirectToAction("Details/" + id.ToString(), "FullTimeEmployees");    
                                //return View("../Admin/Registration"); url not change in browser    
                                if (Session["SelectedPackageID"] == null)
                                {
                                    return RedirectToAction("Index", "Bookings");
                                }
                                else
                                {
                                    return RedirectToAction("Details", "Packages");
                                }
                                

                            }
                            ViewBag.ErrorMessage = "Invallid User Name or Password";
                            return View();
                        }
                        ViewBag.ErrorMessage = "Invallid User Name or Password";
                        return View();
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = " Some database error ocurred, Please try again";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Index", "Packages");
            }
            
        }


        //clear session when logout
        public ActionResult Logout()
        {

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");

        }

        //update the customer infomation 

        public ActionResult Update(int? id)
        {
            //ViewBag.AgentId = new SelectList(db.Customers, "AgentId", "AgtFirstName");
            id = Convert.ToInt32(Session["CustomerID"]);  //an alternative way of getting the id from a session variable.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "CustomerId,CustFirstName,CustLastName,CustAddress,CustCity,CustProv,CustPostal,CustCountry,CustHomePhone,CustBusPhone,CustEmail,AgentId,CustUsername,CustPassword,ConfirmEmail,ConfirmPassword")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.CustPassword = Helper.HashEncrypt(customer.CustPassword);  //hash the new password just before update into the database
                    customer.ConfirmPassword = Helper.HashEncrypt(customer.ConfirmPassword);
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    Session.Clear();
                    Session.Abandon();

                    ViewBag.updateSuccessMessage = "Customer Update completed!!";
                    //ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgtFirstName", customer.AgentId);
                    return RedirectToAction("Login");

                }
                return View(customer);
            }
            catch(Exception e)
            {
                return View(customer);
            }
            
        }
        //this method returns a true or false if email send was successfull
        //
        public bool SendEmail(string toEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = "travelexpertbjmk@gmail.com";
                string senderPassword = "123456Abc";

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);



                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
