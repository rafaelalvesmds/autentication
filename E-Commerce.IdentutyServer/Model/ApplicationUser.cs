﻿using Microsoft.AspNetCore.Identity;

namespace E_Commerce.IdentityServer.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string  FirstName { get; set; }
        public string SecondName { get; set; }
    }
}
