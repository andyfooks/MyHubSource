using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer;
using VirtualTrainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

using System.Configuration;

namespace AJG.VirtualTrainer.Services
{
    public class UserService : BaseService, IDisposable
    {
        public UserService() : base()
        {
        }

        public UserService(IUnitOfWork uow) : base(uow)
        {
        }
        public bool IsUserInRole(string username, string roleName)
        {
            //var user = _unitOfWork.GetRepository<User>().GetAll().FirstOrDefault(et => et.Username == username);
            //return user.UserRoles.Count > 0 && user.UserRoles.Any(r => r.Role.RoleName == RoleEnum.ClaimsHandler.GetDescription() || r.Role.RoleName == RoleEnum.Admin.GetDescription());
            string[] roles = GetRolesForUser(username);
            return roles.Contains(roleName);
        }

        public string[] GetRolesForUser(string username)
        {
            //var user = _unitOfWork.GetRepository<User>().GetAll().FirstOrDefault(et => et.Username == username);
            User user = null;
            using (VirtualTrainerContext ctx = new VirtualTrainerContext())
            {
                user = ctx.User.Where(a => a.DomainName == username || a.Email == username || a.Name == username).FirstOrDefault();

                if (user == null)
                {
                    return new string[0];
                }
                if (user.IsActive)
                    return user.GetAllRoles(ctx).Select(r => r.Name).ToArray();
                else
                    return new string[0];
            }
        }
        public List<User> ListProjectUsersWithPermissions(Guid projectGuid)
        {
            var user2s = (from u in _unitOfWork.Context.User
                          join perms in _unitOfWork.Context.ProjectPermissions on u.Id equals perms.UserId
                          //where perms.ProjectId == projectGuid
                          select new { user = u, permissions = perms } into up
                          group up by up.user.Id into ug
                          select new
                          {
                              User = ug.FirstOrDefault().user,
                              IsInCurrentProject = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.ProjectMember && q.permissions.ProjectId == projectGuid).Any(),
                              IsProjectAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.ProjectAdmin && q.permissions.ProjectId == projectGuid).Any(),
                              IsMicroServiceAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.MicroService && q.permissions.ProjectId == projectGuid).Any(),
                              IsProjectSuperUser = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SuperUser && q.permissions.ProjectId == projectGuid).Any(),
                              ProjectSuperUserInfo = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SuperUser && q.permissions.ProjectId == projectGuid).FirstOrDefault().permissions.Info
                          }).ToList();

            List<User> returnUsers = new List<User>();
            foreach (var u in user2s)
            {
                if (u.IsInCurrentProject)
                {
                    if (u.IsProjectAdmin)
                    {
                        u.User.IsProjectAdmin = true;
                    }
                    if (u.IsMicroServiceAdmin)
                    {
                        u.User.IsMicroServiceMethodAccessUser = true;
                    }
                    if (u.IsProjectSuperUser)
                    {
                        u.User.IsProjectSuperUser = true;
                        u.User.ProjectSuperUserInfo = u.ProjectSuperUserInfo;
                    }
                    returnUsers.Add(u.User);
                }
            }

            return returnUsers;
        }
    }
}
