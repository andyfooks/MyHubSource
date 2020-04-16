using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using AJG.VirtualTrainer;
using System.Diagnostics;

namespace AJG.VirtualTrainer.Helper
{
    public class ADHelper
    {
        #region enums

        public enum DirectoryEntryProeprty
        {
            distinguishedname,
            samaccountname,
            employeeid,
            mail,
            givenname,
            surname,
            name,
            department,
            title,
            manager
        }

        #endregion

        #region [ properties ]

        //private string domainName = string.Empty;
        private string findUserSamAccountName = "(&(sAMAccountName={0}))";
        private string findUserDistinguisedName = "(&(distinguishedname={0}))";
        private string findUsersManagedBySearchFIlter = "(&(objectClass=user)(manager={0}))";
        private string findGroupFilter = "(&(objectclass=group)(name={0}))";
        private string DirectoryEntryPath = string.Empty;
        private List<DirectoryEntryProeprty> DefaultProperties = new List<DirectoryEntryProeprty>();        

        #endregion

        #region [ Constructors ]       
        
        public ADHelper(string DirectoryEntryPath)
        {
            if (string.IsNullOrEmpty(DirectoryEntryPath))            
                throw new ArgumentNullException("The DirectoryEntryPath cannot be null or empty.");
            
            this.DirectoryEntryPath = DirectoryEntryPath;
            SetUpDefaultReturnProperties();
        }

        #endregion

        #region [ Public methods ]                            

        public ADUserDTO GetUser(string userIdentity, bool includeManagees = false, bool recurseManagees = false, bool includeTeamMates = false)
        {
            if (string.IsNullOrEmpty(userIdentity))
            {
                return new ADUserDTO();
            }
            
            // Get User Info
            ADUserDTO user = GetUserInternal(userIdentity);
            if (user != null)
            {
                if (includeManagees)
                {
                    user = recurseUsers(user, recurseManagees);
                    user.HasManagess = user.Managees.Any();
                }
                // if we want team mates then we are going to cheat a bit by iterating over the current user and setting subusers, rather than going to AD which is too slow.
                if (includeTeamMates)
                {
                    // Get manager of user and his managees.
                    ADUserDTO currentUsersManager = GetUserInternal(user.ManagerDetails.SamAccountName);
                    currentUsersManager = recurseUsers(currentUsersManager, false);

                    user.TeammatesSamAccountAndEmployeeIdList = currentUsersManager.Managees.Select(a => new KeyValuePair<string, string>(
                            string.IsNullOrEmpty(a.UserDetails.SamAccountName) ? string.Empty : a.UserDetails.SamAccountName,
                            string.IsNullOrEmpty(a.UserDetails.EmployeeID) ? string.Empty : a.UserDetails.EmployeeID
                        )
                    ).ToList();

                    // Now we iterate the user and set my teammates to managess all way down tree.
                    user = GetTeamMates(user);
                }
            }

            return user;            
        }

        #endregion

        #region [ Private Methods ]

        private void SetUpDefaultReturnProperties()
        {
            this.DefaultProperties.Add(DirectoryEntryProeprty.distinguishedname);
            this.DefaultProperties.Add(DirectoryEntryProeprty.samaccountname);
            this.DefaultProperties.Add(DirectoryEntryProeprty.employeeid);
            this.DefaultProperties.Add(DirectoryEntryProeprty.mail);
            this.DefaultProperties.Add(DirectoryEntryProeprty.givenname);
            this.DefaultProperties.Add(DirectoryEntryProeprty.surname);
            this.DefaultProperties.Add(DirectoryEntryProeprty.name);
            this.DefaultProperties.Add(DirectoryEntryProeprty.department);
            this.DefaultProperties.Add(DirectoryEntryProeprty.title);
            this.DefaultProperties.Add(DirectoryEntryProeprty.manager);
        }
        private ADUserDTO GetUserInternal(string userIdentity)
        {
            ADUserDTO returnDTO = new ADUserDTO();
            
            // If the passed string is null or empty return empty
            if (string.IsNullOrEmpty(userIdentity))
                return returnDTO;

            returnDTO.UserDetails = GetUserDetails(this.findUserSamAccountName, userIdentity);
            if (returnDTO.UserDetails == null)
            {
                return null;
            }

            returnDTO.ManagerDetails = GetUserDetails(this.findUserDistinguisedName, returnDTO.UserDetails.ManagerDistinguishedName);

            return returnDTO;            
        }

        private UserDetails GetUserDetails(string SearchFilter, string userIdentity)
        {
            UserDetails userDetails = null;            
            using (DirectoryEntry searchRoot = new DirectoryEntry(this.DirectoryEntryPath))
            using (DirectorySearcher search = new DirectorySearcher(searchRoot))
            {
                if (!string.IsNullOrEmpty(userIdentity))
                {                    
                    search.Filter = string.Format(SearchFilter, userIdentity);
                    foreach (DirectoryEntryProeprty property in this.DefaultProperties)
                    {
                        search.PropertiesToLoad.Add(property.ToString());
                    }
                                        
                    try
                    {
                        SearchResultCollection results = search.FindAll();
                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult result in results)
                            {                                
                                userDetails = new UserDetails()
                                {
                                    DistinguishedName = result.Properties.Contains(DirectoryEntryProeprty.distinguishedname.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.distinguishedname.ToString()][0] : string.Empty,
                                    EmailAddress = result.Properties.Contains(DirectoryEntryProeprty.mail.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.mail.ToString()][0] : string.Empty,
                                    EmployeeID = result.Properties.Contains(DirectoryEntryProeprty.employeeid.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.employeeid.ToString()][0] : string.Empty,
                                    FullName = result.Properties.Contains(DirectoryEntryProeprty.name.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.name.ToString()][0] : string.Empty,
                                    GivenName = result.Properties.Contains(DirectoryEntryProeprty.givenname.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.givenname.ToString()][0] : string.Empty,
                                    SamAccountName = result.Properties.Contains(DirectoryEntryProeprty.samaccountname.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.samaccountname.ToString()][0] : string.Empty,
                                    Surname = result.Properties.Contains(DirectoryEntryProeprty.surname.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.surname.ToString()][0] : string.Empty,
                                    Department = result.Properties.Contains(DirectoryEntryProeprty.department.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.department.ToString()][0] : string.Empty,
                                    Title = result.Properties.Contains(DirectoryEntryProeprty.title.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.title.ToString()][0] : string.Empty,
                                    ManagerDistinguishedName = result.Properties.Contains(DirectoryEntryProeprty.manager.ToString()) ? (String)result.Properties[DirectoryEntryProeprty.manager.ToString()][0] : string.Empty,
                                };
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        var asa = 11;
                    }
                }
            }

            return userDetails;
        }
        private enum RecurseTarget
        {
            MyTeamMembers,
            Managees
        }
        private ADUserDTO recurseUsers(ADUserDTO user, bool recurse = false)
        {           
            using (DirectoryEntry searchRoot = new DirectoryEntry(DirectoryEntryPath))
            using (DirectorySearcher recursesearch = new DirectorySearcher(searchRoot))
            {
                recursesearch.Filter = string.Format(this.findUsersManagedBySearchFIlter, user.UserDetails.DistinguishedName);
                recursesearch.PropertiesToLoad.Add("samaccountname");
                SearchResultCollection resultCol = recursesearch.FindAll();

                if (resultCol != null)
                {
                    foreach (SearchResult result in resultCol)
                    {
                        ADUserDTO u = this.GetUserInternal(result.Properties.Contains("samaccountname") ? (String)result.Properties["samaccountname"][0] : string.Empty);
                        if (u != null)
                        {
                            if (recurse)
                            {
                                u = recurseUsers(u, recurse);
                            }
                            user.Managees.Add(u);
                        }
                    }
                }
                return user;
            }
        }

        private ADUserDTO GetTeamMates(ADUserDTO user)
        {
            // user.managess.TeamMates = user.Managees
            foreach (var u in user.Managees)
            {
                u.TeammatesSamAccountAndEmployeeIdList = user.Managees.Select(a => new KeyValuePair<string, string>(
                        string.IsNullOrEmpty(a.UserDetails.SamAccountName) ? string.Empty : a.UserDetails.SamAccountName,
                        string.IsNullOrEmpty(a.UserDetails.EmployeeID) ? string.Empty : a.UserDetails.EmployeeID
                    )
                ).ToList();

                GetTeamMates(u);
            }

            return user;            
        }

        #endregion
    }
}
