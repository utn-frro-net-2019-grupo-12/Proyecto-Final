﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPresentationMVC.Models;

namespace WebPresentationMVC.Controllers {

    [Authorize]
    public class HomeController : Controller {
        private readonly IUserSession _userSession;

        public HomeController(IUserSession userSession)
        {
            _userSession = userSession;
        }

        [AllowAnonymous]
        public ActionResult Index() {
            ViewBag.Message = "Hello World.";

            return View();
        }

        public ActionResult Dashboard()
        {
            ViewBag.EmailAddress = _userSession.Username;
            ViewBag.AccessToken = _userSession.BearerToken;

            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
