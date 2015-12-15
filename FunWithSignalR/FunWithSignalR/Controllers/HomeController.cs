using FunWithSignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FunWithSignalR.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View();
        }

        public JsonResult VerifyUserNameInUse(string userName)
        {
            try
            {
                using (var db = new ZigChatContext())
                {
                    return Json(new { Success = true, InUse = db.Connections.Where(x => x.UserName.ToLower() == userName.ToLower() && x.IsOnline).SingleOrDefault() != null }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "Something wrong happened." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}