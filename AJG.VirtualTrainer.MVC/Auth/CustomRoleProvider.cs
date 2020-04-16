using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using AJG.VirtualTrainer.Helper;
using VirtualTrainer.DataAccess;

namespace AJG.VirtualTrainer.MVC.Auth
{
    public class CustomRoleProvider : RoleProvider
    {
        private UserService usersvc = new UserService();

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return usersvc.IsUserInRole(username, roleName);
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = usersvc.GetRolesForUser(username).ToList();
            roles.Add("everyone");
            return roles.ToArray();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}