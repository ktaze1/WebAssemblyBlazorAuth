﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthTest.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string CustomClaim { get; set; }
    }
}
