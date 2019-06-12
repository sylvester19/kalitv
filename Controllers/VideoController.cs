using Kalikoe.ViewModels;
using Kalikoe_BAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Kalikoe.Controllers
{
   
    public class VideoController : Controller
    {
        CRUDVideos objCRUD = new CRUDVideos();
        // GET: Video
        public ActionResult UploadVideos()
        {
            if (Session["userid"] != null)
            {
                SeoDetails seo = new SeoDetails();
                seo = objCRUD.GetSeoDetails("4");
                ViewBag.pagetitle = seo.pagetitle;
                ViewBag.metadesc = seo.pagedescription;
                return View("UploadVideos");
            }
            else
            {
                return Redirect("/login");
            }
        }
        public ActionResult privacypolicy()
        {
            SeoDetails seo = new SeoDetails();
            seo = objCRUD.GetSeoDetails("9");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            return View("privacypolicy");
        }
        public ActionResult termsandconditions()
        {
            SeoDetails seo = new SeoDetails();
            seo = objCRUD.GetSeoDetails("8");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            return View("termsandconditions");
        }
        public ActionResult disclaimer()
        {
            SeoDetails seo = new SeoDetails();
            seo = objCRUD.GetSeoDetails("10");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            return View("disclaimer");
        }
        public ActionResult thankyou()
        {
            return View("thankyou");
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UploadVideos(Videos videoVM, HttpPostedFileBase file1)
        {
            if (Session["userid"] != null)
            {
                string strUploadType = "embed";
                videoVM.access = "1";
                videoVM.categoryid = "1";
                videoVM.embedcode = "";
                if (videoVM.description == null)
                {
                    videoVM.description = "";
                }
               
                if (file1 != null)
                {
                    string strPath = "";
                    string imgurl = string.Empty;
                    string pic = System.IO.Path.GetFileName(file1.FileName);
                    if (objCRUD.Isimage(pic) == true)
                    {
                        strPath = "/image-content/";
                        strUploadType = "image";
                    }
                    else
                    {
                        strPath = "/Videos/";
                        strUploadType = "manual";
                    }
                    strPath = strPath + pic;
                    file1.SaveAs(ConfigurationManager.AppSettings["websitefolderpath"] + strPath);
                    videoVM.videourl = strPath;
                   
                }
                videoVM.uploadtype = strUploadType;
                videoVM.userid = Session["userid"].ToString();
                string strRet = objCRUD.InsertVideos(videoVM);
                ViewBag.success = "Yes";
                return View("UploadVideos");
            }
            else
            {
                return Redirect("/login");
            }
          
        }

        public ActionResult VideoList()
        {

            VideoViewModel videoVM = new VideoViewModel();
            videoVM.listVideosVM = objCRUD.GetAllUserVideos();
            return View("VideoList", videoVM);
        }

        public ActionResult LeaderBoard()
        {
            SeoDetails seo = new SeoDetails();
            seo = objCRUD.GetSeoDetails("2");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            VideoViewModel videoVM = new VideoViewModel();
            videoVM.leaderVM = objCRUD.GetLeaderBoard();
            return View("leaderboard", videoVM);
        }

        public ActionResult Dashboard()
        {
            if (Session["userid"] != null)
            {
                VideoViewModel videoVM = new VideoViewModel();
                videoVM.listVideosVM = objCRUD.Dashboard(Session["userid"].ToString());
                if (Request.QueryString["type"] != null)
                {
                    if (Request.QueryString["type"].ToString() == "first")
                    {
                        ViewBag.showslide = "Yes";
                    }
                    else
                    {
                        ViewBag.showslide = "No";
                    }
                }
                else
                {
                    ViewBag.showslide = "No";
                }
                SeoDetails seo = new SeoDetails();
                seo = objCRUD.GetSeoDetails("3");
                ViewBag.pagetitle = seo.pagetitle;
                ViewBag.metadesc = seo.pagedescription;
                return View("dashboard" , videoVM);
            }
            else
            {
                return Redirect("/login");
            }
        }



        [HttpPost]
        public JsonResult fnLike(updatelikemodel lkVM)
        {
            if (Session["userid"] != null)
            {
                if (lkVM.type == "like")
                {
                    string strRet = objCRUD.InsertLikes(Session["userid"].ToString(), lkVM.vid);
                }
                else if (lkVM.type == "unlike")
                {
                    string strRet = objCRUD.DeleteLikes(Session["userid"].ToString(), lkVM.vid);
                }
                return Json("N", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("E", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult challengeUpdate(challengestatus chVM)
        {          
             string strRet = objCRUD.UpdateChallengeStatus(chVM.challengeid,chVM.status);
             return Json("N", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sponsor(Sponsor spVM)
        {

            spVM.vid = Request.Form["hdnkali"];
            spVM.kaliamount = Request.Form["hdnkaliamt"];
            string strRet = objCRUD.Sponsor(spVM);
            return Redirect(this.Request.UrlReferrer.PathAndQuery + "#category");
        }
        public ActionResult fnRateIt(RateIt rtVM)
        {
            rtVM.vid = Request.Form["hdnvid"];
            rtVM.rating = Request.Form["hdnrating"];
            rtVM.userid = Session["userid"].ToString();
            string strRet = objCRUD.InsertRateIt(rtVM.userid,rtVM.vid,rtVM.rating);
            return Redirect(this.Request.UrlReferrer.PathAndQuery + "#category");
        }
        public ActionResult Comment()
        {           
            string strRet = objCRUD.InsertComments(Request.Form["txtComment"], Request.Form["hdnvid"], Session["userid"].ToString());
            string s1 = this.Request.UrlReferrer.PathAndQuery;
            string s2 = Request.Path.ToString();
            return Redirect(this.Request.UrlReferrer.PathAndQuery + "#comment");
        }
        public ActionResult VideoDetails(string id)
        {
            string strVid = id.Split('-').Last();
            VideoViewModel videoVM = new VideoViewModel();
            videoVM.videoVM = objCRUD.VideoDetails(strVid);
            videoVM.commentVM = objCRUD.GetComments(strVid);
            ViewBag.vid = strVid;
            ViewBag.pagetitle = videoVM.videoVM.displayname;
            ViewBag.metadesc = videoVM.videoVM.displayname;
            return View("videodetails", videoVM);
        }
        public ActionResult ChallengeList()
        {
            if (Session["userid"] != null)
            {
                VideoViewModel videoVM = new VideoViewModel();
                videoVM.chListVM = objCRUD.GetChallengeList(Session["userid"].ToString());
                SeoDetails seo = new SeoDetails();
                seo = objCRUD.GetSeoDetails("11");
                ViewBag.pagetitle = seo.pagetitle;
                ViewBag.metadesc = seo.pagedescription;
                return View("challengelist", videoVM);
            }
            else
            {
                return Redirect("/login");
            }
            
        }
        public ActionResult MyChallengeList()
        {
            if (Session["userid"] != null)
            {
                VideoViewModel videoVM = new VideoViewModel();
                videoVM.chListVM = objCRUD.GetMyChallengeList(Session["userid"].ToString());
                SeoDetails seo = new SeoDetails();
                seo = objCRUD.GetSeoDetails("11");
                ViewBag.pagetitle = seo.pagetitle;
                ViewBag.metadesc = seo.pagedescription;
                return View("challengelist", videoVM);
            }
            else
            {
                return Redirect("/login");
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Challenge(Challenge chVM, HttpPostedFileBase file1)
        {
            if (Session["userid"] != null)
            {
                string strUploadType = "embed";
                chVM.access = "1";
                chVM.categoryid = "1";
                if (Request.Form["chVM.title"] != null)
                {
                    chVM.title = Request.Form["chVM.title"].ToString();
                }
                else
                {
                    chVM.title = "";
                }
                if (Request.Form["chVM.description"] != null)
                {
                    chVM.description = Request.Form["chVM.description"].ToString();
                }
                else
                {
                    chVM.description = "";
                }
                if (Request.Form["chVM.videourl"] != null)
                {
                    chVM.videourl = Request.Form["chVM.videourl"].ToString();
                }
                else
                {
                    chVM.videourl = "";
                }
                if (Request.Form["chVM.tags"] != null)
                {
                    chVM.tags = Request.Form["chVM.tags"].ToString();
                }
                else
                {
                    chVM.tags = "";
                }
                chVM.userid1 = "0";
                chVM.userid2 = Session["userid"].ToString();
                chVM.videoid1 = Request.Form["hdnvid2"].ToString();
                if (Request.Form["ddlvideolist"] !=null & Request.Form["ddlvideolist"] != "0")
                {
                    chVM.videoid2 = Request.Form["ddlvideolist"].ToString();
                }
                else
                {
                    chVM.videoid2 = "0";
                }
                chVM.bidamount = Request.Form["chVM.bidamount"].ToString();
                chVM.biddays = Request.Form["challengetime"].ToString();
                if (file1 != null)
                {
                    string strPath = "";
                    string imgurl = string.Empty;
                    string pic = System.IO.Path.GetFileName(file1.FileName);
                    if (objCRUD.Isimage(pic) == true)
                    {
                        strPath = "/image-content/";
                        strUploadType = "image";
                    }
                    else
                    {
                        strPath = "/Videos/";
                        strUploadType = "manual";
                    }
                    strPath = strPath + pic;
                    file1.SaveAs(ConfigurationManager.AppSettings["websitefolderpath"] + strPath);
                    chVM.videourl = strPath;
                    chVM.uploadtype = strUploadType;
                }
                else
                {
                    chVM.uploadtype = strUploadType;
                }
                string strRet = objCRUD.Challenge(chVM);
                return Redirect(this.Request.UrlReferrer.PathAndQuery + "#category");
            }
            else
            {
                return Redirect("/login");
            }
        }
        public ActionResult Challengethankyou(Challenge chVM)
        {
            return View("Challengethankyou");
        }
        public ActionResult faq()
        {
            SeoDetails seo = new SeoDetails();
            seo = objCRUD.GetSeoDetails("5");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            return View("faq");
        }
        public ActionResult helpandsupport()
        {
            SeoDetails seo = new SeoDetails();
            seo = objCRUD.GetSeoDetails("6");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            return View("helpandsupport");
        }

        [HttpPost]
        public JsonResult DeleteVideos(Videos vm)
        {
            CRUDVideos objCRUD = new CRUDVideos();
            string strRet = objCRUD.DeleteVideoDetails(vm.vid);
            return Json("N", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UserVideoList()
        {
            CRUDVideos objCRUD = new CRUDVideos();
            List<Videos> vidoes = objCRUD.GetUserVideos(Session["userid"].ToString());
            List<ListItem> userlist = new List<ListItem>();
            foreach (var item in vidoes)
            {
                userlist.Add(new ListItem
                {
                    Value = item.vid,
                    Text = item.title
                });
            }
            return Json(userlist);
        }
    }
}