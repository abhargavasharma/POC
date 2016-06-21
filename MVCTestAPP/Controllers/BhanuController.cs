using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCTestAPP.Controllers
{
    public class BhanuController : Controller
    {
        // GET: Bhanu
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Telugu songs
        /// </summary>
        /// <param name="id">Indicvidual track or song ID</param>
        /// <returns></returns>
        public ActionResult Telugu(string id, string type, string lathokre)
        {
             ViewBag.TrackID = (object)id;
             ViewBag.Type = (object)type;
             ViewBag.Pickchakuntla = (object)lathokre;
             return View();
        }

        public ActionResult English(string trackID)
        {
            return View();
        }

        public ActionResult Hindi(string trackID)
        {
            return View();
        }
    }
}