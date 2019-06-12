using Kalikoe_BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kalikoe.ViewModels
{
    public class VideoViewModel
    {
        public List<Videos> listVideosVM { get; set; }
        public List<LeaderBoard> leaderVM { get; set; }
        public List<Videocomments> commentVM { get; set; }
        public Videos videoVM { get; set; }
        public Challenge chVM { get; set; }
        public List<challengelist> chListVM { get; set; }
    }

}