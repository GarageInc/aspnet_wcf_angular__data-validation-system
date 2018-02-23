using System;
using System.Collections.Generic;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Framework.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        public User GetUserById(string userId)
        {
            throw new NotImplementedException();
            //using (var model = new gb_dvsstagingEntities())
            //{
            //    var q = (from user in model.users
            //             where user.Id == userId
            //             select new User
            //             {
            //                 Id = user.Id,
            //                 FirstName = user.FirstName,
            //                 LastName = user.LastName
            //             }).FirstOrDefault();

            //    return q;
            //}
        }

        public User GetUserByName(string userName)
        {
            throw new NotImplementedException();
            //using (var model = new gb_dvsstagingEntities())
            //{
            //    var q = (from user in model.users
            //             where user.UserName == userName
            //             select new User
            //             {
            //                 Id = user.Id,
            //                 FirstName = user.FirstName,
            //                 LastName = user.LastName,
            //                 Email = user.Email
            //             }).FirstOrDefault();

            //    return q;
            //}
        }

        public IList<User> GetUsersByRole(string role)
        {
            throw new NotImplementedException();
            //using (var model = new gb_dvsstagingEntities())
            //{
            //    var q = model.users.Where(u => u.roles.Any(r => r.Name == role)).Select(u => new User {Id = u.Id, Email = u.Email}).ToList();

            //    return q;
            //}
        }
    }
}
