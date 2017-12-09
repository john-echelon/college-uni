//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using CollegeUni.Data.Models;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace CollegeUni.Data.Repos
//{
//    public class AuthRepos : IDisposable
//    {
//        private AuthContext _ctx;

//        private UserManager<IdentityUser> _userManager;

//        public AuthRepos()
//        {
//            _ctx = new AuthContext();
//            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
//        }

//        public async Task<IdentityResult> RegisterUser(UserModel userModel)
//        {
//            IdentityUser user = new IdentityUser
//            {
//                UserName = userModel.UserName
//            };

//            var result = await _userManager.CreateAsync(user, userModel.Password);

//            return result;
//        }

//        public async Task<IdentityUser> FindUser(string userName, string password)
//        {
//            IdentityUser user = await _userManager.FindAsync(userName, password);

//            return user;
//        }

//        public void Dispose()
//        {
//            _ctx.Dispose();
//            _userManager.Dispose();

//        }
//    }
//}
