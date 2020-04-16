using AJG.VirtualTrainer.MVC.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualTrainer;
using static AJG.VirtualTrainer.MVC.Controllers.MicroServiceController;

namespace AJG.VirtualTrainer.MVC
{
    public class MicroServiceActionFilter : ActionFilterAttribute, IActionFilter
    {
        private MicroServiceActionTakenLog log;

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            BaseController c = (BaseController)filterContext.Controller;
            // Check if there is a Micros Services project
            Project MicroServiceproject = c.MicroServiceService.GetMicroServiceProject();
            ServiceResponse customResponse = new ServiceResponse();

            string resultMessage = string.Empty;
            if (MicroServiceproject == null)
            {
                resultMessage = "The Microservices are not configured.";
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotImplemented, resultMessage);
            }
            else
            {
                if (!MicroServiceproject.IsActive)
                {
                    resultMessage = "The Microservices are disabled.";
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotImplemented, resultMessage);
                }
            }

            // Create a new log
            MicroServiceActionTakenLog log = new MicroServiceActionTakenLog(MicroServiceproject);
            log.ActionName = filterContext.ActionDescriptor.ActionName;
            log.UserName = filterContext.HttpContext.User.Identity.Name;
            log.Authenticated = filterContext.HttpContext.User.Identity.IsAuthenticated;
            log.ActionParameters = JsonConvert.SerializeObject(filterContext.ActionParameters);
            log.SessionId = filterContext.HttpContext.Session.SessionID;
            log.Success = false;
            log.ErrorMessage = resultMessage;
            log.Finish = DateTime.Now;

            // Save the log
            this.log = (MicroServiceActionTakenLog)c.MicroServiceService.AddMicroServiceActionLog(log);
        }
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            BaseController c = (BaseController)filterContext.Controller;

            this.log.Success = true;
            if (filterContext.Controller.ViewBag.Success != null && !filterContext.Controller.ViewBag.Success)
            {
                // Gety the Micro Services project
                Project MicroServiceproject = c.MicroServiceService.GetMicroServiceProject();

                string errorMessage = filterContext.Controller.ViewBag.ErrorMessage == null ? string.Empty : filterContext.Controller.ViewBag.Message;
                Exception ex = filterContext.Controller.ViewBag.Exception == null ? new Exception(errorMessage) : filterContext.Controller.ViewBag.Exception;

                // Create a System log
                SystemLog systemLog = new SystemLog(ex);
                systemLog.ProjectID = MicroServiceproject.ProjectUniqueKey;
                systemLog.ProjectDisplayName = MicroServiceproject.ProjectDisplayName;
                systemLog.ProjectName = MicroServiceproject.ProjectName;

                // Associate system error log with activity log.
                this.log.Success = false;
                this.log.ErrorLogEntry = systemLog;
                this.log.ErrorMessage = errorMessage;
            }
            this.log.Finish = DateTime.Now;
            c.MicroServiceService.UpdateMicroServiceActionLog(this.log);
        }
    }
}