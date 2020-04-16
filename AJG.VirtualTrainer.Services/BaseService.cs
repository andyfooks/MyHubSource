using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer.DataAccess;
using VirtualTrainer.Interfaces;

namespace AJG.VirtualTrainer.Services
{
    public class BaseService
    {
        //protected ExecutionSettings _execSettings;
        protected UnitOfWork _unitOfWork;

        public BaseService()
        {
            //  _execSettings = new ExecutionSettings(ConfigurationManager.ConnectionStrings["K2"].ToString());
            //this._execSettings.UseImpersonation = true;

            _unitOfWork = new UnitOfWork();
        }

        public BaseService(IUnitOfWork uow)
        {
            _unitOfWork = uow as UnitOfWork;
        }

        public void Dispose()
        {
            //_unitOfWork.Dispose();
        }
    }
}
