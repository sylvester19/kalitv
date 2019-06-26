using CaptchaMvc.HtmlHelpers;
using Facebook;
using Kalikoe.ViewModels;
using Kalikoe_BAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kalikoe.Controllers
{
    public class HomeController : Controller
    {
        CRUDUsers objUsr = new CRUDUsers();
        CRUDVideos objVideo = new CRUDVideos();
        public string test()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/");
        }
        public ActionResult Index()
        {
            HomeViewModel videoVM = new HomeViewModel();
            string strUserId = "0";
            if (Session["userid"] != null)
            {
                strUserId = Session["userid"].ToString();
            }
            if (Request.QueryString["tagname"] != null)
            {
                videoVM.listVideosVM = objVideo.GetTagVideos(Request.QueryString["tagname"].ToString(), strUserId);
            }
            else if (Request.QueryString["searchvalue"] != null)
            {
                videoVM.listVideosVM = objVideo.GetSearchVideos(Request.QueryString["searchvalue"].ToString(), strUserId);
            }
            else
            {

                if (Session["userid"] != null)
                {
                    strUserId = Session["userid"].ToString();
                }
                videoVM.listVideosVM = objVideo.GetAllUserVideos(strUserId);
            }
            videoVM.strSponsor = objVideo.GetSponsorVideo();
            videoVM.listTagsVM = objVideo.GetTagList();

            SeoDetails seo = new SeoDetails();
            seo = objVideo.GetSeoDetails("1");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;

            return View("home", videoVM);
        }
        public ActionResult IndexNew()
        {
            HomeViewModel videoVM = new HomeViewModel();
            if (Request.QueryString["tagname"] != null)
            {
                videoVM.listVideosVM = objVideo.GetTagVideos(Request.QueryString["tagname"].ToString());
            }
            else
            {
                videoVM.listVideosVM = objVideo.GetAllUserVideos();
            }
            videoVM.strSponsor = objVideo.GetSponsorVideo();
            videoVM.listTagsVM = objVideo.GetTagList();
            return View("homenew", videoVM);
        }
        public ActionResult CaptchaIndex()
        {
            string[] strArray = new string[36];
            strArray = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            Random autoRand = new Random();
            string strCaptcha = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                int j = Convert.ToInt32(autoRand.Next(0, 62));
                strCaptcha += strArray[j].ToString();
            }
            Session["Captcha"] = strCaptcha;
            ImageConverter converter = new ImageConverter();
            // Response.BinaryWrite((byte[])converter.ConvertTo(CaptchaGeneration(strCaptcha), typeof(byte[])));
            Response.Write(Convert.ToBase64String((byte[])converter.ConvertTo(CaptchaGeneration(strCaptcha), typeof(byte[]))));
            return View();
        }
        public string ValidateCaptcha(string Code)
        {
            try
            {
                if (Session["Captcha"].ToString() == Code)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            catch { return "0"; }
        }
        public Bitmap CaptchaGeneration(string captchatxt)
        {
            Bitmap bmp = new Bitmap(133, 48);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                Font font = new Font("Tahoma", 14);
                graphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, bmp.Width, bmp.Height);
                graphics.DrawString(captchatxt, font, new SolidBrush(Color.Gold), 25, 10);
                graphics.Flush();
                font.Dispose();
                graphics.Dispose();
            }
            return bmp;
        }

        [HttpPost]
        public ActionResult SignUp(Users userVM)
        {
            if (this.IsCaptchaValid("Validate your captcha"))
            {
                int strRet = objUsr.UserSignUp(userVM);
                if (strRet == 1)
                {
                    string strHtml = objUsr.ReadSingup();
                    string strURL = ConfigurationManager.AppSettings["domainname"] + "users/confirmation/";
                    strURL += objUsr.encrypt(userVM.emailaddress);
                    strHtml = strHtml.Replace("$Link$", strURL);
                    strHtml = strHtml.Replace("$username$", userVM.emailaddress);
                    bool blSendEmail = objVideo.sendemail(userVM.emailaddress, "kalikoeteam@gmail.com", "Kalikoe - Signup Confirmation", strHtml, true);
                    ViewBag.success = "Yes";
                    return View("test");
                }
                else
                {
                    ViewBag.unsuccess = "NO";
                    ViewBag.captchamsg = "";
                    return View("test");
                }
            }
            else
            {
                ViewBag.captchamsg = "Invalid Captcha";
                return View("test");
            }

        }
        public JsonResult User_Check(exusers usercheck)
        {
            CRUDUsers obj = new CRUDUsers();
            int intval = obj.UserSignUp_Check(usercheck);

            return Json(intval, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ConfimrUser(string id)
        {
            string strRet = objUsr.ConfirmUser(objUsr.decrypt(id.ToString()));
            return View("emailconfirmation");
        }

        [HttpPost]
        public ActionResult Profiles(Users userVM, HttpPostedFileBase file1)
        {

            string strImgPath = "/images/user/";
            string imgurl = string.Empty;
            if (file1 != null)
            {
                string pic = System.IO.Path.GetFileName(file1.FileName);
                strImgPath = strImgPath + pic;
                file1.SaveAs(ConfigurationManager.AppSettings["websitefolderpath"] + strImgPath);
                userVM.userlogo = strImgPath;
            }
            userVM.aboutme = Request.Form["userVM.aboutme"].ToString();
           // userVM.gender = Request.Form["gender"].ToString();
            userVM.userid = Session["userid"].ToString();
            string strRet = objUsr.ProfileUpdate(userVM);
            Session["displayname"] = userVM.firstname + " " + userVM.lastname;
            if (strRet == "U")
            {
                return Redirect("/profile?msg=update");
            }
            else
            {
                return View("profile");
            }
        }

        public ActionResult Profiles()
        {
            if (Session["userid"] != null)
            {
                UsersViewModel usrVM = new UsersViewModel();
                usrVM.countrylistVM = objUsr.GetCountry();
                usrVM.genderlistVM = objUsr.GetGender();
                usrVM.userVM = objUsr.ProfileDisplay(Session["userid"].ToString());
                ViewBag.kalidollar = objUsr.ProfileDollar(Session["userid"].ToString());

                return View("profile", usrVM);
            }
            else
            {
                return Redirect("/login");
            }

        }

        public ActionResult UserProfiles(string id)
        {
            string strUserId = id.Split('-').Last();
            UsersViewModel usrVM = new UsersViewModel();
            usrVM.userVM = objUsr.ProfileDisplay(strUserId);
            return View("userprofile", usrVM);
        }

        public ActionResult Login()
        {
            if (Request.QueryString["type"] != null)
            {
                Session["userid"] = null;
                Session["displayname"] = null;
            }
            if (Session["userid"] != null)
            {
                return Redirect("/dashboard");
            }
            else
            {
                return View("login");
            }
        }

        [HttpPost]
        public ActionResult Login(Users userVM)
        {
            Users uVM = new Users();
            uVM = objUsr.LoginCheck(userVM);
            if (this.IsCaptchaValid("Validate your captcha"))
            {
                if (uVM.userid != null)
                {
                    Session["userid"] = uVM.userid;
                    Session["encemailaddress"] = uVM.encemailaddress;
                    Session["displayname"] = uVM.displayname;
                    Session["challengecount"] = objUsr.challengecount(Session["userid"].ToString());
                    Session["userlogo"] = uVM.userlogo;
                    return Redirect("/");
                }
                else
                {
                    return Redirect("/login?val=false");
                }
            }
            else
            {
                ViewBag.captchamsg = "Invalid Captcha";
                return View("login");
            }
        }

        [HttpPost]
        public ActionResult passwordupdate(Users userVM)
        {
            string strRet = objUsr.updatepassword(userVM.userid, userVM.password);
            return Redirect("/login");
        }
        public ActionResult SignUp()
        {
            return View("test");
        }
        public ActionResult thankyou()
        {
            return View("thankyou");
        }
        public ActionResult forgot()
        {
            return View("forgot");
        }

        [HttpPost]
        public ActionResult ForgotPassword(Users userVM)
        {
            if (objUsr.CheckUserName(userVM.emailaddress) == false)
            {
                return Redirect("/forgot-password?msg=invaild");
            }
            else
            {
                string strRet = objUsr.ReadForgotHtml();
                string strURL = ConfigurationManager.AppSettings["domainname"] + "reset-password/";
                strURL += objUsr.encrypt(userVM.emailaddress);
                strRet = strRet.Replace("$Link$", strURL);
                bool blSendEmail = objVideo.sendemail(userVM.emailaddress, "kalikoeteam@gmail.com", "Reset Password - Kalikoe", strRet, true);
                return Redirect("/forgot-msg");
            }
        }

        public ActionResult ResetPassword(string id)
        {
            TempData["encryemail"] = objUsr.decrypt(id.ToString());
            return View("resetpassword");
        }

        [HttpPost]
        public ActionResult ResetPassword(Users userVM)
        {
            if (userVM.password != userVM.confirmpassword)
            {
                return Redirect("/reset-password?msg=invaild");
            }
            else
            {
                string strRet = objUsr.ResetPassword(TempData["encryemail"].ToString(), userVM.password);
                return Redirect("/reset-msg");
            }
        }
        public ActionResult forgotmsg()
        {
            return View("forgotmsg");
        }
        public ActionResult resetmsg()
        {
            return View("resetmsg");
        }
        public ActionResult ContactUs()
        {
            SeoDetails seo = new SeoDetails();
            seo = objVideo.GetSeoDetails("7");
            ViewBag.pagetitle = seo.pagetitle;
            ViewBag.metadesc = seo.pagedescription;
            return View("contactus");
        }
        [HttpPost]
        public ActionResult ContactUs(contactus contactusVM)
        {
            if (this.IsCaptchaValid("Validate your captcha"))
            {
                if (contactusVM.message == null)
                {
                    contactusVM.message = "";
                }
                string strRet = objUsr.InsertContactus(contactusVM);
                ViewBag.success = "Yes";
                return View("contactus");
            }
            else
            {
                ViewBag.captchamsg = "Invalid Captcha";
                return View("contactus");
            }
        }
        [HttpPost]
        public JsonResult fnFollow(followermodel flVM)
        {
            if (flVM.type == "Follow")
            {
                string strRet = objUsr.InsertFollowers(flVM.userid, Session["userid"].ToString());
            }
            else if (flVM.type == "Unfollow")
            {
                string strRet = objUsr.DeleteFollowers(flVM.userid, Session["userid"].ToString());
            }

            return Json("N", JsonRequestBehavior.AllowGet);
        }
        //thaya
        //facebook login
        private Uri RediredtUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        private Uri RediredtUri1
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookSignCallback");
                return uriBuilder.Uri;
            }
        }

        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "388494591743514",
                client_secret = "300d7cf149dd54ee51bbc8d46d63a00f",
                redirect_uri = RediredtUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        [AllowAnonymous]
        public ActionResult FacebookSignUp()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "399027980938795",
                client_secret = "f34a6e726701849f9db49eb2f4969ece",
                redirect_uri = RediredtUri1.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "388494591743514",
                client_secret = "300d7cf149dd54ee51bbc8d46d63a00f",
                redirect_uri = RediredtUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;
            dynamic obj = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
            string email = obj.email;
            var ds = objUsr.Google_FB_UserInsert(obj.email, obj.first_name, obj.last_name, obj.first_name + " " + obj.last_name, obj.id, obj.gender);
            Session["userid"] = ds.Tables[0].Rows[0]["userid"].ToString();
            Session["encemailaddress"] = ds.Tables[0].Rows[0]["emailaddress"].ToString();
            Session["displayname"] = ds.Tables[0].Rows[0]["firstname"].ToString() + " " + ds.Tables[0].Rows[0]["lastname"].ToString();
            Session["challengecount"] = objUsr.challengecount(Session["userid"].ToString());
            FormsAuthentication.SetAuthCookie(email, false);
            return Redirect("/dashboard?type=first");
        }

        public ActionResult FacebookSignCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "399027980938795",
                client_secret = "f34a6e726701849f9db49eb2f4969ece",
                redirect_uri = RediredtUri1.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            fb.AccessToken = accessToken;
            dynamic obj = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
            string email = obj.email;
            var ds = objUsr.Signup_Google_FB_UserInsert(obj.email, obj.first_name, obj.last_name, obj.first_name + " " + obj.last_name, obj.id, obj.gender);
            FormsAuthentication.SetAuthCookie(email, false);
            ViewBag.userid = ds.Tables[0].Rows[0]["userid"].ToString();
            if (ds.Tables[0].Rows[0]["type"].ToString() == "new")
            {
                return View("signuppassword");
            }
            else
            {
                return Redirect("/login");
            }
        }


        //google login

        private string ClientId = ConfigurationManager.AppSettings["Google.ClientID"];
        private string SecretKey = ConfigurationManager.AppSettings["Google.SecretKey"];
        private string RedirectUrl = ConfigurationManager.AppSettings["Google.RedirectUrl"];

        private string ClientId1 = ConfigurationManager.AppSettings["Google.ClientID1"];
        private string SecretKey1 = ConfigurationManager.AppSettings["Google.SecretKey1"];
        private string RedirectUrl1 = ConfigurationManager.AppSettings["Google.RedirectUrl1"];

        public void LoginUsingGoogle()
        {
            Response.Redirect($"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId}&response_type=code&scope=openid%20email%20profile&redirect_uri={RedirectUrl}&state=abcdef");
        }

        public void SignupUsingGoogle()
        {

            Response.Redirect($"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId1}&response_type=code&scope=openid%20email%20profile&redirect_uri={RedirectUrl1}&state=abcdef");
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            Session["user"] = null;
            return View("Index");
        }

        /// <summary>  
        /// Listen response from Google API after user authorization  
        /// </summary>  
        /// <param name="code">access code returned from Google API</param>  
        /// <param name="state">A value passed by application to prevent Cross-site request forgery attack</param>  
        /// <param name="session_state">session state</param>  
        /// <returns></returns>  
        [HttpGet]
        public async Task<ActionResult> SaveGoogleUser(string code, string state, string session_state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            var requestUrl = $"oauth2/v4/token?code={code}&client_id={ClientId}&client_secret={SecretKey}&redirect_uri={RedirectUrl}&grant_type=authorization_code";

            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            var response = await httpClient.SendAsync(req);
            var token = JsonConvert.DeserializeObject<GmailToken>(await response.Content.ReadAsStringAsync());
            Session["user"] = token.AccessToken;
            var obj = await GetuserProfile(token.AccessToken);
            var sd = obj.Email;
            var ds = objUsr.Google_FB_UserInsert(obj.Email, obj.GivenName, obj.FamilyName, obj.Name, obj.Id, obj.Gender);
            ds.Tables[0].Rows[0]["userid"].ToString();

            Session["userid"] = ds.Tables[0].Rows[0]["userid"].ToString();
            Session["encemailaddress"] = ds.Tables[0].Rows[0]["emailaddress"].ToString();
            Session["displayname"] = ds.Tables[0].Rows[0]["firstname"].ToString() + " " + ds.Tables[0].Rows[0]["lastname"].ToString();
            Session["challengecount"] = objUsr.challengecount(Session["userid"].ToString());
            return Redirect("/dashboard?type=first");
            //return View("UserProfile", obj);
        }

        [HttpGet]
        public async Task<ActionResult> SignupGoogleUser(string code, string state, string session_state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            var requestUrl = $"oauth2/v4/token?code={code}&client_id={ClientId1}&client_secret={SecretKey1}&redirect_uri={RedirectUrl1}&grant_type=authorization_code";

            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            var response = await httpClient.SendAsync(req);
            var token = JsonConvert.DeserializeObject<GmailToken>(await response.Content.ReadAsStringAsync());
            var obj = await GetuserProfile(token.AccessToken);
            var sd = obj.Email;
            var ds = objUsr.Signup_Google_FB_UserInsert(obj.Email, obj.GivenName, obj.FamilyName, obj.Name, obj.Id, obj.Gender);
            ViewBag.userid = ds.Tables[0].Rows[0]["userid"].ToString();
            if (ds.Tables[0].Rows[0]["type"].ToString() == "new")
            {
                return View("signuppassword");
            }
            else
            {
                return Redirect("/login");
            }

        }

        /// <summary>  
        /// To fetch User Profile by access token  
        /// </summary>  
        /// <param name="accesstoken">access token</param>  
        /// <returns>User Profile page</returns>  
        public async Task<UserProfile> GetuserProfile(string accesstoken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            string url = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={accesstoken}";
            var response = await httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<UserProfile>(await response.Content.ReadAsStringAsync());
        }

        public ActionResult KalikoeMaster()
        {
            HomeViewModel videoVM = new HomeViewModel();
            if (Request.QueryString["tagname"] != null)
            {
                videoVM.listVideosVM = objVideo.GetTagVideos(Request.QueryString["tagname"].ToString());
            }
            else
            {
                videoVM.listVideosVM = objVideo.GetAllUserVideos();
            }
            videoVM.strSponsor = objVideo.GetSponsorVideo();
            videoVM.listTagsVM = objVideo.GetTagList();
            return View("KalikoeMaster", videoVM);
        }

        public ActionResult _LeftPanel()
        {
            HomeViewModel videoVM = new HomeViewModel();
            CRUDBlog objBlog = new CRUDBlog();
            if (Request.QueryString["tagname"] != null)
            {
                videoVM.listVideosVM = objVideo.GetTagVideos(Request.QueryString["tagname"].ToString());
            }
            else
            {
                videoVM.listVideosVM = objVideo.GetAllUserVideos();
            }
            videoVM.strSponsor = objVideo.GetSponsorVideo();
            videoVM.listTagsVM = objVideo.GetTagList();
            videoVM.bloglistVM = objBlog.recentbloglist();
            return View("_LeftPanel", videoVM);
        }

        public ActionResult _RightPanel()
        {
            HomeViewModel videoVM = new HomeViewModel();
            CRUDBlog objBlog = new CRUDBlog();
            videoVM.bloglistVM = objBlog.recentbloglist();
            return View("_RightPanel", videoVM);
        }
        public ActionResult KalikoeMaster1()
        {

            return View();
        }


    }
}