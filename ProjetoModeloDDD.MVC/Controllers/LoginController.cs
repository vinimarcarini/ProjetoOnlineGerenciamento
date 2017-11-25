using AutoMapper;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoModeloDDD.MVC.Views
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult VerificarEmailNoServidor(string Email)
        {
            bool emailExiste = false;
            try
            {
                emailExiste = Email.Equals("macoratti@yahoo.com") ? true : false;
                return Json(!emailExiste, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}