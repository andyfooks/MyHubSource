using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer;
using VirtualTrainer.Interfaces;

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

                user = ctx.User.Include(x => x.UserRoles).Include("UserRoles.Role").AsNoTracking().FirstOrDefault(et => et.Username == username);

                if (user == null)
                {
                    return new string[0];
                }
            }

            if (user.IsActive)
                return user.UserRoles.Select(r => r.Role.RoleName).ToArray();
            else
                return new string[0];
        }

        //public Boolean CheckUserExists(string username)
        //{

        //    return _unitOfWork.GetRepository<User>().Find(x => x.Username == username).Any();
        //}


        //public void AddUser(UserDto userDto)
        //{
        //    User user = new User()
        //    {
        //        Name = userDto.Name,
        //        Username = userDto.Username,
        //        TeamId = userDto.TeamId,
        //        IsActive = (bool)userDto.IsActive,
        //        ReactivatedOn = userDto.ReactivatedOn,
        //        DeactivatedOn = userDto.DeactivatedOn
        //    };

        //    //TODO: Add roles
        //    _unitOfWork.GetRepository<User>().Create(user);

        //    if (userDto.IsAdmin.HasValue ? (bool)userDto.IsAdmin : false)
        //        user.UserRoles.Add(new UserRole() { RoleId = 1, UserId = user.Id });

        //    if (userDto.IsManager.HasValue ? (bool)userDto.IsManager : false)
        //        user.UserRoles.Add(new UserRole() { RoleId = 2, UserId = user.Id });

        //    if (userDto.IsClaimsHandler.HasValue ? (bool)userDto.IsClaimsHandler : false)
        //        user.UserRoles.Add(new UserRole() { RoleId = 3, UserId = user.Id });

        //    _unitOfWork.Commit();
        //}

        //public IQueryable<User> GetClaimHandlers(bool activeOnly = true)
        //{
        //    //3	Claims Handler
        //    return _unitOfWork.GetRepository<User>().GetAll(u => u.UserRoles).Where(e => e.IsActive == true && e.UserRoles.Any(r => r.RoleId == 3));

        //}

        //public void DeleteUser(int Id)
        //{
        //    _unitOfWork.GetRepository<User>().Delete(Id);
        //    _unitOfWork.Commit();
        //}

        //public void UpdateUser(UserDto userDto)
        //{
        //    User user = _unitOfWork.GetRepository<User>().Single(x => x.Username == userDto.Username);

        //    user.Name = userDto.Name;
        //    user.TeamId = userDto.TeamId;
        //    user.Username = userDto.Username;

        //    UserRole adminUser = _unitOfWork.GetRepository<UserRole>().Single(x => x.RoleId == 1 && x.UserId == user.Id);
        //    UserRole managerUser = _unitOfWork.GetRepository<UserRole>().Single(x => x.RoleId == 2 && x.UserId == user.Id);
        //    UserRole claimsHandlerUser = _unitOfWork.GetRepository<UserRole>().Single(x => x.RoleId == 3 && x.UserId == user.Id);
        //    UserRole operationalUser = _unitOfWork.GetRepository<UserRole>().Single(x => x.RoleId == 4 && x.UserId == user.Id);
        //    UserRole readOnlyUser = _unitOfWork.GetRepository<UserRole>().Single(x => x.RoleId == 5 && x.UserId == user.Id);

        //    if (userDto.IsActive.HasValue)
        //    {
        //        if (!(bool)userDto.IsActive && user.IsActive)
        //        {
        //            DateTime now = DateTime.Now;
        //            user.DeactivatedOn = now;
        //            userDto.DeactivatedOn = now;
        //        }

        //        else if ((bool)userDto.IsActive && !user.IsActive)
        //        {
        //            if ((bool)userDto.IsActive && !user.IsActive)
        //            {
        //                user.DeactivatedOn = null;
        //                userDto.DeactivatedOn = null;
        //                user.ReactivatedOn = DateTime.Now;
        //            }
        //        }
        //    }

        //    user.IsActive = (bool)userDto.IsActive;

        //    if (userDto.IsAdmin.HasValue)
        //    {
        //        if ((bool)userDto.IsAdmin && adminUser == null)
        //            user.UserRoles.Add(new UserRole() { UserId = userDto.Id ?? default(int), RoleId = 1 });

        //        else if ((bool)!userDto.IsAdmin && adminUser != null)
        //            _unitOfWork.GetRepository<UserRole>().Delete(adminUser);
        //    }
        //    if (userDto.IsManager.HasValue)
        //    {
        //        if ((bool)userDto.IsManager && managerUser == null)
        //            user.UserRoles.Add(new UserRole() { UserId = userDto.Id ?? default(int), RoleId = 2 });
        //        else if (!(bool)userDto.IsManager && managerUser != null)
        //            _unitOfWork.GetRepository<UserRole>().Delete(managerUser);
        //    }
        //    if (userDto.IsClaimsHandler.HasValue)
        //    {
        //        if ((bool)userDto.IsClaimsHandler && claimsHandlerUser == null)
        //            user.UserRoles.Add(new UserRole() { UserId = userDto.Id ?? default(int), RoleId = 3 });
        //        else if ((bool)!userDto.IsClaimsHandler && claimsHandlerUser != null)
        //            _unitOfWork.GetRepository<UserRole>().Delete(claimsHandlerUser);
        //    }
        //    if (userDto.IsOperationalUser.HasValue)
        //    {
        //        if ((bool)userDto.IsOperationalUser && operationalUser == null)
        //            user.UserRoles.Add(new UserRole() { UserId = userDto.Id ?? default(int), RoleId = 4 });
        //        else if ((bool)!userDto.IsOperationalUser && operationalUser != null)
        //            _unitOfWork.GetRepository<UserRole>().Delete(operationalUser);
        //    }
        //    if (userDto.ReadOnly.HasValue)
        //    {
        //        if ((bool)userDto.ReadOnly && readOnlyUser == null)
        //            user.UserRoles.Add(new UserRole() { UserId = userDto.Id ?? default(int), RoleId = 5 });
        //        else if ((bool)!userDto.ReadOnly && readOnlyUser != null)
        //            _unitOfWork.GetRepository<UserRole>().Delete(readOnlyUser);
        //    }
        //    _unitOfWork.GetRepository<User>().Update(user);
        //    _unitOfWork.Commit();
        //}

        //public List<ADUserDto> GetADUsers()
        //{
        //    List<ADUserDto> users;

        //    PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

        //    //var dn = "CN=Alex Williams,OU=System Applications,OU=London,OU=HLG Users,OU=HLG - HeathLambert,OU=Locations,OU=IBEU,DC=emea,DC=ajgco,DC=com";
        //    //var cn = dn.Split(',').Where(i => i.Contains("DC=")).Select(i => i.Replace("DC=", "")).FirstOrDefault();

        //    using (var context = new PrincipalContext(ContextType.Domain))
        //    {
        //        string adGroupName = ConfigurationManager.AppSettings["ClaimsTeamCompleteADName"];
        //        using (var group = GroupPrincipal.FindByIdentity(context, adGroupName))
        //        {
        //            users = group.GetMembers().Select(x => new ADUserDto()
        //            {
        //                Name = x.Name,
        //                Username = x.SamAccountName.Contains("\\") ? x.SamAccountName : x.DistinguishedName.Split(',').Where(i => i.Contains("DC=")).Select(i => i.Replace("DC=", "")).FirstOrDefault() + "\\" + x.SamAccountName,
        //                Display = string.Format("{0} - {1}", x.SamAccountName, x.Name)
        //            }).ToList();
        //        }
        //    }

        //    return users;
        //}

        //public IQueryable<User> GetUsers()
        //{
        //    return _unitOfWork.GetRepository<User>().GetAll();
        //}

        //public IQueryable<User> GetActiveUsers()
        //{
        //    return _unitOfWork.GetRepository<User>().GetAll().Where(e => e.IsActive == true);
        //}

        //public User GetCurrentUserByUsername(string username)
        //{
        //    //return _unitOfWork.GetRepository<User>().Single(x => x.Username == username);
        //    return _unitOfWork.Context.User.Include(et => et.Team).FirstOrDefault(x => x.Username == username && x.IsActive == true);
        //}
        //public User GetUserByUsername(string username)
        //{
        //    //return _unitOfWork.GetRepository<User>().Single(x => x.Username == username);
        //    return _unitOfWork.Context.User.Include(et => et.Team).FirstOrDefault(x => x.Username == username);
        //}
        //public Team GetTeamByTeamId(int id)
        //{
        //    return _unitOfWork.GetRepository<Team>().GetAll().Where(et => et.Id == id).FirstOrDefault();
        //}

        //public string GetUserTeam(int userId)
        //{
        //    var user = _unitOfWork.GetRepository<User>().GetAll(u => u.Team).Where(et => et.Id == userId).FirstOrDefault();
        //    if (user != null)
        //    {
        //        return user.Team.Name;
        //    }
        //    return "";

        //}
        ///* Do not add active users condition to this method because is being user in the ActivityLogService.cs */
        //public User GetCurrentUserById(int id)
        //{
        //    //return _unitOfWork.GetRepository<User>().Single(x => x.Username == username);
        //    return _unitOfWork.Context.User.Include(et => et.Team).FirstOrDefault(x => x.Id == id);
        //}


        //public bool IsUserInRole(string username, string roleName)
        //{
        //    //var user = _unitOfWork.GetRepository<User>().GetAll().FirstOrDefault(et => et.Username == username);
        //    //return user.UserRoles.Count > 0 && user.UserRoles.Any(r => r.Role.RoleName == RoleEnum.ClaimsHandler.GetDescription() || r.Role.RoleName == RoleEnum.Admin.GetDescription());
        //    string[] roles = GetRolesForUser(username);
        //    return roles.Contains(roleName);
        //}

        //public string[] GetRolesForUser(string username)
        //{
        //    //var user = _unitOfWork.GetRepository<User>().GetAll().FirstOrDefault(et => et.Username == username);
        //    User user = null;
        //    using (AJGClaimsContext ctx = new AJGClaimsContext())
        //    {

        //        user = ctx.User.Include(x => x.UserRoles).Include("UserRoles.Role").AsNoTracking().FirstOrDefault(et => et.Username == username);

        //        if (user == null)
        //        {
        //            return new string[0];
        //        }
        //    }

        //    if (user.IsActive)
        //        return user.UserRoles.Select(r => r.Role.RoleName).ToArray();
        //    else
        //        return new string[0];
        //}

        //public void Dispose()
        //{
        //    base.Dispose();
        //}
    }
}
