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
using System.Data.Entity.Infrastructure;
using AJG.VirtualTrainer.Helper.Encryption;
using AJG.VirtualTrainer.Helper.Exchange;

namespace AJG.VirtualTrainer.Services
{
    public class MicroServiceService : BaseService, IDisposable
    {
        public MicroServiceService() : base()
        {
        }

        public MicroServiceService(IUnitOfWork uow) : base(uow)
        {
        }

        #region [ Logs ]

        public MicroServiceActionTakenLog AddMicroServiceActionLog(MicroServiceActionTakenLog log)
        {
            // If there is a microService project - get the ID and associate this with it.
            Project project = _unitOfWork.Context.Project.Where(p => p.IsMicroServicesProject == true).FirstOrDefault();
            if (project != null)
            {
                log.Project = project;
                log.ProjectId = project.ProjectUniqueKey;
                log.ProjectName = project.ProjectName;
                log.ProjectDisplayName = project.ProjectDisplayName;
                log.ProjectDescription = project.ProjectDescription;
            }
            _unitOfWork.Context.ActionTakenLogs.Add(log);
            _unitOfWork.Context.SaveChanges();
            return log;
        }
        public MicroServiceActionTakenLog UpdateMicroServiceActionLog(MicroServiceActionTakenLog log)
        {
            // If there is a microService project - get the ID and associate this with it.
            Project project = _unitOfWork.Context.Project.Where(p => p.IsMicroServicesProject == true).FirstOrDefault();
            if (project != null)
            {
                log.Project = project;
                log.ProjectId = project.ProjectUniqueKey;
                log.ProjectName = project.ProjectName;
                log.ProjectDisplayName = project.ProjectDisplayName;
                log.ProjectDescription = project.ProjectDescription;
            }
            _unitOfWork.Context.SaveChanges();
            return log;
        }

        #endregion


        #region [ Other ]

        public string GetEnvironment()
        {
            return ConfigurationManager.AppSettings[AppSettingsEnum.targetSystem.ToString()];
        }
        public string GetExchangeRuleEnabled()
        {
            string enabled = ConfigurationManager.AppSettings[AppSettingsEnum.ExchangeRuleEnabled.ToString()];
            return string.IsNullOrEmpty(enabled) ? "false" : enabled;
        }
        public string GetExcelRuleEnabled()
        {
            string enabled = ConfigurationManager.AppSettings[AppSettingsEnum.ExcelRuleEnabled.ToString()];
            return string.IsNullOrEmpty(enabled) ? "false" : enabled;
        }
        public string GetEncryptionKey()
        {
            string returnString = ConfigurationManager.AppSettings[AppSettingsEnum.EncryptionKey.ToString()];
            return string.IsNullOrEmpty(returnString) ? "" : returnString;
        }

        #endregion

        #region [ Project ]

        public Project GetMicroServiceProject()
        {
            return _unitOfWork.GetRepository<Project>().GetAllNoTrack().Where(p => p.IsMicroServicesProject == true).FirstOrDefault();
        }

        #endregion

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
