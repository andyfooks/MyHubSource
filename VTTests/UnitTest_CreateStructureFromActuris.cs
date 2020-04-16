using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualTrainer;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.IO;
using RazorEngine.Templating;
using System.Text;
using System.Net.Mail;
using AJG.VirtualTrainer.Services;
using VirtualTrainer.Utilities;

namespace VTTests
{
    [TestClass]
    public class CreateVTStructureFromActurisInstance
    {
        private string VirtualTrainerProjectsId = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";
        
        [TestMethod]
        public void CreateBusinessStructure ()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Project project = ctx.Project.Where(a => a.ProjectUniqueKey == new Guid(VirtualTrainerProjectsId)).FirstOrDefault();
                project.ExecuteAllActurisBusinessStructureConfigurations(ctx);
                //ProcessStructureFromActuris("[VirtualTrainer].[dbo].[GetUserBusinessStructureAJGLloyds]");
            }
        }
    }
}
