using Kalikoe_BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kalikoe.ViewModels
{
    public class UsersViewModel
    {
        public Users userVM { get; set; }
        public exusers usercheck { get; set; }
        public List<country> countrylistVM { get; set; }
    }
}