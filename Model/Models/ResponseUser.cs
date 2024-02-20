﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ResponseUser
    {
        public string userName { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string zip { get; set; }
        public string role { get; set; }
        public string city { get; set; }
        public string accessToken { get; set; }
    }
}
