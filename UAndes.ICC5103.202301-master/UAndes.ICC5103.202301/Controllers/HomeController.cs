﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UAndes.ICC5103._202301.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";
            return View();
        }
    }
}
