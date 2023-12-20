using PassportManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace PassportManagementSystem.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]//Prevents Page from going back when back button is pressed
    public class PassportManagementSystemController : Controller
    {
        //Displays Home View
        public ActionResult Home()
        {
            return View();
        }
        //Displays About View
        public ActionResult About()
        {
            return View();
        }
        //Displays Contact View
        public ActionResult Contact()
        {
            return View();
        }
        //Displays Register View
        public ActionResult Register()
        {
            return View();
        }
        //When user submits the form in register page it validates
        //If validation is successfull then it goes to DBOperations Class and
        //fetches the data and store in ViewBag.data(used in .cshtml to display) and empty the fiels in the form
        //Else it returns to the same view with validation messages
        [HttpPost]
        public ActionResult Register(UserRegistration R)
        {
            R.ApplyType = "Passport User";
            ModelState.Remove("UserID");
            ModelState.Remove("ApplyType");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                UserRegistration details = DBOperations.Registration(R);
                ViewBag.data = details;
                ModelState.Clear();
                return View();
            }
            else
                return View();
        }
        
                //Displays Login View
        //Session is used to remove that particular session when user logouts
        public ActionResult Login(String message=null)
        {
            if (Session["EmailAddress"] != null && Session["ApplyType"] != null && Session["UserID"] != null)
            {
                Session.Abandon();
                Session.Clear();
            }
            if(message != null)
            {
                ViewBag.success = message;
            }
            return View();
        }
        //When user submits the form in Login page it validates
        //If validation is successfull then it goes to DBOperations Class and 
        //fetches the data and session is created for userid and applytype
        //based on applytype it redirects to that particular page
        //Else it returns to the same view with validation messages
        [HttpPost]
        public ActionResult Login(UserRegistration R)
        {         
            if (ModelState.IsValidField("EmailAddress") && ModelState.IsValidField("Password"))
            {
                UserRegistration userdetails = DBOperations.Login(R);
                if(userdetails!=null)
                {
                    Session["EmailAddress"] = userdetails.EmailAddress;
                    Session["ApplyType"] = userdetails.ApplyType;
                    Session["UserID"] = userdetails.UserID;
                    if (userdetails.ApplyType == "Passport User")
                    {
                        Session["Welcome"] = userdetails.FirstName + " " + userdetails.LastName;
                        return RedirectToAction("ApplyPassport");
                    }
                    else if (userdetails.ApplyType == "Admin")
                    {
                        Session["Welcome"] = userdetails.FirstName + " " + userdetails.LastName;
                        return RedirectToAction("ApplicantDetails");
                    }
                }
                else
                {
                    ViewBag.error = "Invalid Credentials";
                    ModelState.Clear();
                    return View();
                }
                return View();
            }
            else
                return View();
        }
        //when user logins if user type is passport then it redirects to this Action
        //DBOperations fetches state data on page load to view inorder to select state by the user
        //Used Session to restrict users to directly access the link without login
        public ActionResult ApplyPassport()
        {
            if (Session["EmailAddress"] == null && Session["ApplyType"] == null && Session["UserID"] == null)
                return RedirectToAction("Login");
            ViewBag.UserID = Session["UserID"];
            return View();
        }
        //When user apply the passport it validates
        //If validation is successfull then it goes to DBOperations Class and
        //fetches the data and sends data or error messages to view
        [HttpPost]

        public ActionResult ApplyPassport([Bind(Exclude = "ProofOfCitizenship, Photo, BirthCertificate")] PassportApplication P, HttpPostedFileBase ProofOfCitizenship, HttpPostedFileBase Photo, HttpPostedFileBase BirthCertificate)
        {
            ModelState.Remove("ReasonForReIssue");
            ModelState.Remove("PassportNumber");
            ModelState.Remove("ProofOfCitizenship");
            ModelState.Remove("Photo");
            ModelState.Remove("BirthCertificate");
            ModelState.Remove("Status");
            ModelState.Remove("Comments");
            if (ModelState.IsValid)
            {
                ViewBag.UserID = Session["UserID"];
                if (ProofOfCitizenship != null)
                {
                    int filelength = ProofOfCitizenship.ContentLength;
                    byte[] Myfile_1 = new byte[filelength];
                    ProofOfCitizenship.InputStream.Read(Myfile_1, 0, filelength);
                    P.ProofOfCitizenship = Myfile_1;
                }
                else
                {
                    ViewBag.error = "Invalid File Format, Upload only PDF files";
                }

                if (Photo != null)
                {
                    int filelength = Photo.ContentLength;
                    byte[] Myfile_2 = new byte[filelength];
                    Photo.InputStream.Read(Myfile_2, 0, filelength);
                    P.Photo = Myfile_2;
                }
                else
                {
                    ViewBag.error = "Invalid File Format, Upload only JPEG files";
                }

                if (BirthCertificate != null)
                {
                    int filelength = BirthCertificate.ContentLength;
                    byte[] Myfile_3 = new byte[filelength];
                    BirthCertificate.InputStream.Read(Myfile_3, 0, filelength);
                    P.BirthCertificate = Myfile_3;
                }
                else
                {
                    ViewBag.error = "Invalid File Format, Upload only PDF files";
                }

                PassportApplication details = DBOperations.ApplyPassport(P);
                if (details != null)
                    ViewBag.data = details;
                else
                    ViewBag.error = "Passport Number w.r.t UserID already generated";
                ModelState.Clear();
                return View();
            }
            else
            {
                ViewBag.UserID = Session["UserID"];
                return View();
            }
        }
        public FileResult DownloadFile(String userID, string identifier)
        {
            ApplicantDetails applicantDetails = DBOperations.Applicant(userID);
            byte[] bytes;
            if (identifier == "POC")
            {
                bytes = applicantDetails.passports.ProofOfCitizenship;
                return File(bytes, "application/pdf", "Proof_Of_Citizenship_" + userID + ".pdf");
            }
            if (identifier == "BC")
            {
                bytes = applicantDetails.passports.BirthCertificate;
                return File(bytes, "application/pdf", "Birth_Certificate_" + userID + ".pdf");
            }
            return null;
        }
        public ActionResult ApplicationStatus(String userID)
        {
            if (Session["UserID"] == null && Session["ApplyType"] == null && Session["EmailAddress"] == null)
                return RedirectToAction("Login");
            ViewBag.UserID = Session["UserID"];
            ApplicantDetails applicantDetails = DBOperations.Applicant(userID);
            return View(applicantDetails);
        }
         public ActionResult ForgotPassword()
        {
            if (Session["EmailAddress"] != null)
            {
                Session.Abandon();
                Session.Clear();
            }
            return View();
        }

        //Displays Passport ReIssue View
        //DBOperations fetches state data on page load to view inorder to select state by the user
        //Used Session to restrict users to directly access the link without login
        public ActionResult PassportReIssue()
        {
            if (Session["EmailAddress"] == null && Session["ApplyType"] == null && Session["UserID"] == null)
                return RedirectToAction("Login");
            ViewBag.UserID = Session["UserID"];
            return View();
        }
        //When user submit the passport data for reissue it validates
        //If validation is successfull then it goes to DBOperations Class and 
        //fetches the data and sends data or error messages to view 
        [HttpPost]
        public ActionResult PassportReIssue([Bind(Exclude = "ProofOfCitizenship, Photo, BirthCertificate")] PassportApplication P, HttpPostedFileBase ProofOfCitizenship, HttpPostedFileBase Photo, HttpPostedFileBase BirthCertificate)
        {
            ModelState.Remove("ProofOfCitizenship");
            ModelState.Remove("Photo");
            ModelState.Remove("BirthCertificate");
            ModelState.Remove("Status");
            ModelState.Remove("Comments");
            if (ModelState.IsValid)
            {
                ViewBag.UserID = Session["UserID"];
                if (ProofOfCitizenship != null)
                {
                    int filelength = ProofOfCitizenship.ContentLength;
                    byte[] Myfile_1 = new byte[filelength];
                    ProofOfCitizenship.InputStream.Read(Myfile_1, 0, filelength);
                    P.ProofOfCitizenship = Myfile_1;
                }
                else
                {
                    ViewBag.error = "Invalid File Format, Upload only PDF files";
                }

                if (Photo != null)
                {
                    int filelength = Photo.ContentLength;
                    byte[] Myfile_2 = new byte[filelength];
                    Photo.InputStream.Read(Myfile_2, 0, filelength);
                    P.Photo = Myfile_2;
                }
                else
                {
                    ViewBag.error = "Invalid File Format, Upload only JPEG files";
                }

                if (BirthCertificate != null)
                {
                    int filelength = BirthCertificate.ContentLength;
                    byte[] Myfile_3 = new byte[filelength];
                    BirthCertificate.InputStream.Read(Myfile_3, 0, filelength);
                    P.BirthCertificate = Myfile_3;
                }
                else
                {
                    ViewBag.error = "Invalid File Format, Upload only PDF files";
                }
                PassportApplication details = DBOperations.PassportReIssue(P);
                if (details != null)
                    ViewBag.data = details;
                else
                    ViewBag.error = "Passport Number w.r.t UserId doesn't exists";
                ModelState.Clear();
                return View();
            }
            else
            {
                ViewBag.UserID = Session["UserID"];
                return View();
            }
        }

         //When user submits the security question and answer then it validates
        //If validation is successfull then it goes to DBOperations Class and 
        //returns 'Success' and removes the 'Authentication' session and redirects to VisaCancellation
        //else returns to same view
        [HttpPost]
        public ActionResult ForgotPassword(UserRegistration U)
        {
            if (ModelState.IsValidField("SecurityQuestion") && ModelState.IsValidField("SecurityAnswer") && ModelState.IsValidField("EmailAddress"))
            {
                string authentication = DBOperations.ForgotPassword(U);
                if(authentication!= "Success")
                {
                    ModelState.Clear();
                    ViewBag.error = authentication;
                    return View();
                }
                else
                { 
                    ModelState.Clear();
                    Session["EmailAddress"] = U.EmailAddress;
                    return RedirectToAction("ResetPassword");
                }
            }
            else
                return View();
        }
        
        public ActionResult ResetPassword()
        {
            if (Session["EmailAddress"] == null)
                return RedirectToAction("Login");
            return View();
        }
        //When user submits the security question and answer then it validates
        //If validation is successfull then it goes to DBOperations Class and 
        //returns 'Success' and removes the 'Authentication' session and redirects to VisaCancellation
        //else returns to same view
        [HttpPost]
        public ActionResult ResetPassword([Bind(Exclude = "EmailAddress")] UserRegistration U)
        {
            if (ModelState.IsValidField("Password") && ModelState.IsValidField("ConfirmPassword"))
            {
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("EmailAddress");
                U.EmailAddress = (string)Session["EmailAddress"];
                string resetpassword = DBOperations.ResetPassword(U);
                if (resetpassword != "Success")
                {
                    ModelState.Clear();
                    ViewBag.error = resetpassword;
                    return View();
                }
                else
                {
                    ModelState.Clear();
                    Session.Abandon();
                    Session.Clear();
                    return RedirectToAction("Login",null, new { message = "Password Updated Successfully"});
                }
            }
            else
                return View();
        }
        public ActionResult ApplicantDetails()
        {
            if (Session["UserID"] == null && Session["ApplyType"] == null && Session["EmailAddress"] == null)
                return RedirectToAction("Login");
            ViewBag.UserID = Session["UserID"];
            List<ApplicantDetails> applicantDetails = DBOperations.ApplicantDetails();
            return View(applicantDetails);
        }
        public ActionResult Applicant(String userID)
        {
            if (Session["UserID"] == null && Session["ApplyType"] == null && Session["EmailAddress"] == null)
                return RedirectToAction("Login");
            ViewBag.UserID = Session["UserID"];
            ApplicantDetails applicantDetails = DBOperations.Applicant(userID);
            return View(applicantDetails);
        }
        [HttpPost]
        public ActionResult Applicant(String userID, String passportNum, FormCollection form)
        {
            String status = form["Status"];
            String comments = form["Comments"];
            ApplicantDetails applicantDetails = DBOperations.ApplicantStatus(userID, passportNum, status, comments);
            ViewBag.data = applicantDetails;
            return View(applicantDetails);
        }
    }
}
