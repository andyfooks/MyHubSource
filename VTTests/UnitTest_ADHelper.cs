using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AJG.VirtualTrainer.Helper;
using System.Diagnostics;
using System.Collections.Generic;
using System.Timers;
using VirtualTrainer;
using System.Linq;
using System.Data.Entity;
using AJG.VirtualTrainer.Services;
using VirtualTrainer.DataAccess;

namespace VTTests
{
    [TestClass]
    public class UnitTest_ADHelper
    {
        private enum UserNames
        {
            chcrawford,            
            davedwards,
            smatson,
            mrea,
            gbirch,
            sipearce,
            jwcoughlan,
            npallam,
            hagrawal,
            alowson,
            cneden,
            afooks,
            adacosta
        }
        private enum AJGDomains
        {
            emea,
            corp
        }
        public void SaveADStructure()
        {
            
        }
        [TestMethod]
        public void CheckMobilePhoneUsersWithWrongId()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                var result = from c in ctx.PhoneSummaryActivity
                             group c by c.UserName into user
                             select user;

                var uow = new UnitOfWork();
                AdminService s = new AdminService(uow);
                var aduser = s.GetAdUserFromDB(UserNames.chcrawford.ToString());

                foreach (var u in result)
                {
                    var user = aduser.GetSpecifcUserFromHierachyUserName(u.Key);
                    if(user != null)
                    {
                        var q = u.FirstOrDefault();
                        if(user.UserDetails.EmployeeID != q.EmployeeID)
                        {
                            Console.WriteLine(string.Format("User Name: {0}, AD Id = {1}, MP Employee ID = {2}", u.Key, user.UserDetails.EmployeeID, q.EmployeeID));
                        }
                    }
                }
            }           
        }

        [TestMethod]
        public void TestADuserMemberOFGroup()
        {
            ADHelper helper = new ADHelper("GC://DC=ajgco,DC=com");

            //bool ismember = helper.IsMemberOfGroup("everyone", UserNames.afooks.ToString(), AJGDomains.emea.ToString());
        }
        [TestMethod]
        public void TestGetDTOForUser()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ADHelper helper = new ADHelper("GC://DC=ajgco,DC=com");
            ADUserDTO result = helper.GetUser(UserNames.cneden.ToString(), true, false);
            Assert.IsNotNull(result);
            sw.Stop();
            int subcount = result.GetUserSubordinatesList().Count;
            string elapsed = sw.Elapsed.ToString();
        }
        [TestMethod]
        public void GetAllPropertiesForAdUser()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //ADHelper helper = new ADHelper("emea");
            //var result = helper.GetAllPropertiesForUser("afooks");
            //foreach(var r in result)
            //{
            //    Console.WriteLine(string.Format("{0}:{1}", r.Key, r.Value));
            //}
            //Assert.IsNotNull(result);
            //sw.Stop();
            //string elapsed = sw.Elapsed.ToString();            
        }
        [TestMethod]
        public void Test_PeopleMangedBy()
        {
            //ADHelper helper = new ADHelper("emea");
            //ADHelper.ADUserDTO result = helper.GetUser(UserNames.glippolis.ToString(), true);

            //Assert.IsNotNull(result);
        }
        [TestMethod]
        public void Test_PeopleMangedByRecurse()
        {
            //ADHelper helper = new ADHelper("emea");
            //ADHelper.ADUserDTO result = helper.GetUser(UserNames.stodonnell.ToString(), true, true);

            //Assert.IsNotNull(result);
        }        
    }
}
