using Kalikoe.ViewModels;
using Kalikoe_BAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using Facebook;
using System.Web.Security;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Kalikoe.Controllers
{
    public class BlogController : Controller
    {
        CRUDBlog objblog = new CRUDBlog();

        public ActionResult BlogList()
        {
            BlogViewModel blVM = new BlogViewModel();
            blVM.bloglistVM = objblog.bloglist();
            ViewBag.pagetitle = "Kali Tv | Blog List";
            ViewBag.metadesc = "Kali Tv Blog List";
            return View("bloglist", blVM);
        }

        public ActionResult BlogDetails(string id)
        {
            string strVid = id.Split('-').Last();           
            BlogViewModel blVM = new BlogViewModel();
            blVM.blogVM = objblog.blogdetails(strVid);
            ViewBag.pagetitle = blVM.blogVM.pagetitle;
            ViewBag.metadesc = blVM.blogVM.pagedescription;
            return View("BlogDetails", blVM);
        }
    }
}