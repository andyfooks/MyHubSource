using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper
{
    public class UserDetails : ICloneable
    {
        public string DistinguishedName { get; set; }
        public string SamAccountName { get; set; }
        public string EmployeeID { get; set; }
        public string EmailAddress { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string ManagerDistinguishedName { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class ADUserDTO
    {
        public string UserSummaryInfo
        {
            get
            {
                return string.Format("SamAccountName: {0}, Email: {1}, Title: {2}, Department: {3}, Manager: {4}", 
                    this.id,
                    this.UserDetails!= null ? this.UserDetails.EmailAddress : "",
                    this.UserDetails != null ? this.UserDetails.Title : "",
                    this.UserDetails != null ? this.UserDetails.Department : "",
                    this.ManagerDetails != null ? this.ManagerDetails.FullName : "");
            }
        }
        public string UserDisplayName
        {
            get
            {
                return string.IsNullOrEmpty(UserDetails.FullName) ? UserDetails.SamAccountName : UserDetails.FullName;
            }
        }
        public string id
        {
            get
            {
                return UserDetails.SamAccountName;
            }
        }
        public bool HasManagess { get; set; }
        public UserDetails UserDetails { get; set; }
        public List<ADUserDTO> Managees { get; set; }
        public UserDetails ManagerDetails { get; set; }      
        public bool expanded { get; set; }
        public bool selected { get; set; }
        
        public string GetUniqueIdentifier()
        {
            return string.IsNullOrEmpty(this.UserDetails.EmployeeID) ? this.UserDetails.SamAccountName : this.UserDetails.EmployeeID;
        }
        public bool HasTeamMember(string userSamAccountName)
        {
            return this.Managees.Where(s => string.Compare(s.UserDetails.SamAccountName, userSamAccountName, true) == 0).Any();
        }
        public bool HasSubordinate(string userSamAccountName)
        {
            return this.GetSpecifcUserFromHierachy(userSamAccountName) == null ? false : true;
        }
        public List<string> GetSubordinateUpperHierachy(string UserToFind)
        {
            var returnUsersSamAccountNames = new List<string>();
            // If the user 
            if (this.HasSubordinate(UserToFind))
            {
                var targetUser = this.GetSpecifcUserFromHierachy(UserToFind);
                returnUsersSamAccountNames.Add(targetUser.UserDetails.SamAccountName);

                while (targetUser.UserDetails.SamAccountName != this.UserDetails.SamAccountName)
                {
                    // Get the manager
                    targetUser = this.GetSpecifcUserFromHierachy(targetUser.ManagerDetails.SamAccountName);
                    returnUsersSamAccountNames.Add(targetUser.UserDetails.SamAccountName);
                }

                returnUsersSamAccountNames.Reverse();
            }

            return returnUsersSamAccountNames;
        }
        public List<ADUserDTO> GetFlattened()
        {
            List<ADUserDTO> returnList = new List<ADUserDTO>();

            returnList.AddRange(this.Managees.Select(m => new ADUserDTO()
            {
                ManagerDetails = m.ManagerDetails,
                UserDetails = m.UserDetails,
                HasManagess = m.HasManagess
            }).ToList());            

            foreach (var user in this.Managees)
            {
                returnList.AddRange(user.GetFlattened());
            }

            return returnList;
        }
        public ADUserDTO GetSpecifcUserFromHierachy(string identifyer)
        {
            // Is it this object?
            if(string.Compare(UserDetails.SamAccountName, identifyer, true) == 0 || string.Compare(UserDetails.EmployeeID, identifyer, true) == 0)
            {
                return this;
            }

            // recursively check all managees etc
            foreach(var managee in Managees)
            {
                var ret = managee.GetSpecifcUserFromHierachy(identifyer);

                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
        public List<ADUserDTO> FindMatchingHierachyUsers(string searchString)
        {
            var returnUsers = new List<ADUserDTO>();

            // Is it this object?
            if (string.Compare(UserDetails.SamAccountName, searchString, true) == 0 || 
                string.Compare(UserDetails.EmployeeID, searchString, true) == 0 || 
                UserDetails.FullName.ToLower().Contains(searchString.ToLower()))
            {
                returnUsers.Add(this);
            }

            // recursively check all managees etc
            foreach (var managee in Managees)
            {
                var ret = managee.FindMatchingHierachyUsers(searchString);
                returnUsers.AddRange(ret);
            }

            return returnUsers;
        }
        public ADUserDTO GetSpecifcUserFromHierachyUserName(string userName)
        {
            // Is it this object?
            if (string.Compare(UserDetails.FullName, userName, true) == 0)
            {
                return this;
            }

            // recursively check all managees etc
            foreach (var managee in Managees)
            {
                var ret = managee.GetSpecifcUserFromHierachyUserName(userName);

                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
        public ADUserDTO GetUserAndManageesOnly()
        {
            this.HasManagess = this.Managees.Any();
            // just want to destroy managee of managees
            foreach(var managee in this.Managees)
            {
                managee.HasManagess = managee.Managees.Any();
                managee.Managees = new List<ADUserDTO>();
            }
            return this;
        }
        public ADUserDTO GetUserOnly()
        {
            this.HasManagess = this.Managees.Any();
            // just want to destroy managee of managees

            this.Managees = new List<ADUserDTO>();

            return this;
        }
        public List<KeyValuePair<string, string>> TeammatesSamAccountAndEmployeeIdList { get; set; }
        public List<KeyValuePair<string, string>> UserAndManageesSamAccountAndEmployeeIdList()
        {
            List<KeyValuePair<string, string>> returnData = ManageesSamAccountAndEmployeeIdList();
            returnData.Add(new KeyValuePair<string, string>(
                string.IsNullOrEmpty(this.UserDetails.SamAccountName) ? string.Empty : this.UserDetails.SamAccountName,
                string.IsNullOrEmpty(this.UserDetails.EmployeeID) ? string.Empty : this.UserDetails.EmployeeID
            ));
            return returnData;
        }       
        public List<KeyValuePair<string, string>> ManageesSamAccountAndEmployeeIdList()
        {
            List<KeyValuePair<string, string>> returnData = new List<KeyValuePair<string, string>>();
            returnData = this.Managees.Select(a => new KeyValuePair<string, string>(
                    string.IsNullOrEmpty(a.UserDetails.SamAccountName) ? string.Empty : a.UserDetails.SamAccountName,
                    string.IsNullOrEmpty(a.UserDetails.EmployeeID) ? string.Empty : a.UserDetails.EmployeeID
                )
            ).ToList();

            return returnData;
        }
        public List<KeyValuePair<string, string>> UserAndSubordinatesSamAccountAndEmployeeIdList()
        {
            List<KeyValuePair<string, string>> returnData = SubordinatesSamAccountAndEmployeeIdList();
            returnData.Add(new KeyValuePair<string, string>(
                string.IsNullOrEmpty(this.UserDetails.SamAccountName) ? string.Empty : this.UserDetails.SamAccountName,
                string.IsNullOrEmpty(this.UserDetails.EmployeeID) ? string.Empty : this.UserDetails.EmployeeID
            ));
            return returnData;
        }
        public List<KeyValuePair<string, string>> SubordinatesSamAccountAndEmployeeIdList()
        {
            List<KeyValuePair<string, string>> returnData = new List<KeyValuePair<string, string>>();
            returnData = RecurseUserForSubordinatesAccountEmployeeIDList(this);
            return returnData;
        }
        public List<string> GetUserAndUserSubordinatesList()
        {
            List<string> returnList = this.GetUserSubordinatesList();
            returnList.Insert(0, this.UserDetails.SamAccountName);
            return returnList;
        }
        public List<string> GetUserSubordinatesList()
        {
            List<string> returnList = RecurseUserForSubordinatesList(this);
            return returnList;
        }
        public string GetUserAndUserSubordinates_CommaSeperatedString()
        {            
            return string.Join(",", GetUserAndUserSubordinatesList());
        }
        public string GetUserSubordinates_CommaSeperatedString()
        {
            return string.Join(",", GetUserSubordinatesList());
        }

        public List<string> GetUserAndUserTeamList()
        {
            List<string> returnList = GetUserTeamList();
            returnList.Insert(0, this.UserDetails.SamAccountName);
            return returnList;
        }
        public List<string> GetUserTeamList()
        {
            List<string> returnList = this.Managees.Select(a => a.UserDetails.SamAccountName).ToList();
            return returnList;
        }
        public List<string> GetUserTeamEmployeeIdList()
        {
            List<string> returnList = this.Managees.Select(a => a.UserDetails.EmployeeID).ToList();
            return returnList;
        }
        public List<string> GetUserAndUserTeamEmployeeIdList()
        {
            List<string> returnList = this.Managees.Select(a => a.UserDetails.EmployeeID).ToList();
            returnList.Insert(0, this.UserDetails.EmployeeID);
            return returnList;
        }
        public string GetUserAndUserTeam_CommaSeperatedString()
        {
            return string.Join(",", GetUserAndUserTeamList());     
        }
        public string GetUserTeam_CommaSeperatedString()
        {
            return string.Join(",", GetUserTeamList());
        }
        public List<string> GetUserTeammatesList()
        {
            List<string> returnList = this.TeammatesSamAccountAndEmployeeIdList.Select(a => a.Key).ToList();
            return returnList;
        }
        public string GetUserTeammates_CommaSeperatedString()
        {
            return string.Join(",", GetUserTeammatesList());
        }
        public ADUserDTO()
        {
            this.Managees = new List<ADUserDTO>();
            this.TeammatesSamAccountAndEmployeeIdList = new List<KeyValuePair<string, string>>();
            ManagerDetails = new UserDetails();
        }

        #region [ Private Methods ]

        private List<string> RecurseUserForSubordinatesList(ADUserDTO user)
        {
            List<string> returnlist = new List<string>();

            if (user.Managees.Any())
            {
                returnlist = user.Managees.Select(a => a.UserDetails.SamAccountName).ToList();

                foreach (var managee in user.Managees)
                {
                    returnlist.AddRange(RecurseUserForSubordinatesList(managee));
                }
            }
            return returnlist;
        }

        private List<KeyValuePair<string,string>> RecurseUserForSubordinatesAccountEmployeeIDList(ADUserDTO user)
        {
            List<KeyValuePair<string, string>> returnlist = new List<KeyValuePair<string, string>>();

            if (user.Managees.Any())
            {
                returnlist = user.Managees.Select(a => new KeyValuePair<string, string>(
                        string.IsNullOrEmpty(a.UserDetails.SamAccountName) ? string.Empty : a.UserDetails.SamAccountName,
                        string.IsNullOrEmpty(a.UserDetails.EmployeeID) ? string.Empty : a.UserDetails.EmployeeID
                    )
                ).ToList();

                foreach (var managee in user.Managees)
                {
                    returnlist.AddRange(RecurseUserForSubordinatesAccountEmployeeIDList(managee));
                }
            }
            return returnlist;
        }

        #endregion
    }   
}
