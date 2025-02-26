﻿using E_Commerce.IdentityServer.Model;
using E_Commerce.IdentityServer.Model.Context;
using E_Commerce.IdentuiyServer.Configuration.Initializer;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace E_Commerce.IdentityServer.Configuration.Initializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly MySQLContext _context;
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole> _role;

        public DBInitializer(MySQLContext context,
            UserManager<ApplicationUser> user,
            RoleManager<IdentityRole> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        public void Initialize()
        {
            if (_role.FindByNameAsync(IdentityConfiguration.Admin).Result != null) return;
            _role.CreateAsync(new IdentityRole(
                IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _role.CreateAsync(new IdentityRole(
              IdentityConfiguration.Client)).GetAwaiter().GetResult();

            ApplicationUser admin = new ApplicationUser()
            {
                UserName = "rafael-admin",
                Email = "rafaelalvesmds@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (31) 12345-6789",
                FirstName = "Rafael",
                SecondName = "Admin"
            };

            _user.CreateAsync(admin, "Rafael@2022").GetAwaiter().GetResult();
            _user.AddToRoleAsync(admin,
                IdentityConfiguration.Admin).GetAwaiter().GetResult();

            var adminClaims = _user.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.SecondName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.SecondName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin),

            }).Result;

            ApplicationUser client = new ApplicationUser()
            {
                UserName = "rafael-client",
                Email = "rafaelalvesmds@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (31) 12345-6789",
                FirstName = "Rafael",
                SecondName = "Client"
            };

            _user.CreateAsync(client, "Rafael@2022").GetAwaiter().GetResult();
            _user.AddToRoleAsync(client,
                IdentityConfiguration.Client).GetAwaiter().GetResult();

            var clientClaims = _user.AddClaimsAsync(client, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.SecondName}"),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.FamilyName, client.SecondName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client),

            }).Result;
        }

    }
}
