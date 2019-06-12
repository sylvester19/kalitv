using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kalikoe
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "homepagename",
              url: "homepage",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
            name: "homepagenamenew",
            url: "homepagenew",
            defaults: new { controller = "Home", action = "IndexNew", id = UrlParameter.Optional }
        );

            routes.MapRoute(
           name: "signupname",
           url: "signup",
           defaults: new { controller = "Home", action = "signUp", id = UrlParameter.Optional }
       );

            routes.MapRoute(
        name: "challengelistname",
        url: "challengelist",
        defaults: new { controller = "Video", action = "ChallengeList", id = UrlParameter.Optional }
    );


            routes.MapRoute(
          name: "loginname",
          url: "login",
          defaults: new { controller = "Home", action = "login", id = UrlParameter.Optional }
      );

            routes.MapRoute(
                name: "thankyouname",
                url: "thank-you",
                defaults: new { controller = "Home", action = "thankyou", id = UrlParameter.Optional }
            );
            routes.MapRoute(
      name: "resetname",
      url: "reset-password/{id}",
      defaults: new { controller = "Home", action = "ResetPassword", id = UrlParameter.Optional }
  );

            routes.MapRoute(
      name: "userwconfimration",
      url: "users/confirmation/{id}",
      defaults: new { controller = "Home", action = "ConfimrUser", id = UrlParameter.Optional }
  );

            routes.MapRoute(
      name: "resetmsgname",
      url: "reset-msg",
      defaults: new { controller = "Home", action = "resetmsg", id = UrlParameter.Optional }
  );

            routes.MapRoute(
        name: "forgotname",
        url: "forgot-password",
        defaults: new { controller = "Home", action = "forgot", id = UrlParameter.Optional }
    );

            routes.MapRoute(
        name: "forgotmsgname",
        url: "forgot-msg",
        defaults: new { controller = "Home", action = "forgotmsg", id = UrlParameter.Optional }
    );

            routes.MapRoute(
       name: "privacypolicyname",
       url: "privacy-policy",
       defaults: new { controller = "Video", action = "privacypolicy", id = UrlParameter.Optional }
   );
            routes.MapRoute(
      name: "termsandconditionsname",
      url: "terms-and-conditions",
      defaults: new { controller = "Video", action = "termsandconditions", id = UrlParameter.Optional }
  );
            routes.MapRoute(
      name: "disclaimername",
      url: "disclaimer",
      defaults: new { controller = "Video", action = "disclaimer", id = UrlParameter.Optional }
  );

            routes.MapRoute(
         name: "thankyouvideo",
         url: "video-thankyou",
         defaults: new { controller = "Video", action = "thankyou", id = UrlParameter.Optional }
     );

            routes.MapRoute(
        name: "uploadvideoname",
        url: "UploadVideos",
        defaults: new { controller = "Video", action = "UploadVideos", id = UrlParameter.Optional }
    );

            routes.MapRoute(
        name: "VideoListname",
        url: "VideoList",
        defaults: new { controller = "Video", action = "VideoList", id = UrlParameter.Optional }
    );

            routes.MapRoute(
        name: "leaderboardname",
        url: "leaderboard",
        defaults: new { controller = "Video", action = "LeaderBoard", id = UrlParameter.Optional }
    );
            routes.MapRoute(
       name: "dashboardname",
       url: "dashboard",
       defaults: new { controller = "Video", action = "Dashboard", id = UrlParameter.Optional }
   );

            routes.MapRoute(
       name: "profilename",
       url: "profile",
       defaults: new { controller = "Home", action = "Profiles", id = UrlParameter.Optional }
   );

            routes.MapRoute(
      name: "videodetailsname",
      url: "details/{id}",
      defaults: new { controller = "Video", action = "VideoDetails", id = UrlParameter.Optional }
  );
            routes.MapRoute(
            name: "challenge-thankyou-name",
            url: "challenge-thankyou",
            defaults: new { controller = "Video", action = "Challengethankyou", id = UrlParameter.Optional }
         );

            routes.MapRoute(
           name: "userprofilename",
           url: "user-detail/{id}",
           defaults: new { controller = "home", action = "UserProfiles", id = UrlParameter.Optional }
        );

            routes.MapRoute(
       name: "faqname",
       url: "faq",
       defaults: new { controller = "Video", action = "faq", id = UrlParameter.Optional }
   );

            routes.MapRoute(
       name: "helpandsupportname",
       url: "help-and-support",
       defaults: new { controller = "Video", action = "helpandsupport", id = UrlParameter.Optional }
   );

            routes.MapRoute(
      name: "contactusname",
      url: "contact-us",
      defaults: new { controller = "Home", action = "ContactUs", id = UrlParameter.Optional }
  );

            routes.MapRoute(
     name: "bloglistname",
     url: "bloglist",
     defaults: new { controller = "Blog", action = "BlogList", id = UrlParameter.Optional }
 );

            routes.MapRoute(
     name: "blogdetailsname",
     url: "blog-details/{id}",
     defaults: new { controller = "Blog", action = "BlogDetails", id = UrlParameter.Optional }
 );

            routes.MapRoute(
    name: "mychallangename",
    url: "mychallenge",
    defaults: new { controller = "Video", action = "MyChallengeList", id = UrlParameter.Optional }
);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
