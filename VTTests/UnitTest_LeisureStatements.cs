using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Data.Entity;
using AJG.VirtualTrainer.Helper.Excel;
using System.Collections.Generic;
using AJG.VirtualTrainer.Helper;

namespace VTTests
{
    [TestClass]
    public class UnitTest_LeisureStatements
    {
        [TestMethod]
        public void PopulateLeisureStatementTablesWithDataFromExcel()
        {
            using (var ctx = new VTTestContext())
            {
                System.IO.DirectoryInfo directoryName = System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
                string DirectoryFullPath = directoryName.Parent.FullName;
                string docFullPath = string.Format("{0}\\LeisureStatements.xls", DirectoryFullPath);
                string oledbConnectionString = ConfigurationHelper.Get(AppSettingsList.oledbConnectionString);
                //PopulateIntroducerCodesTable(ctx, docFullPath);
                //PopulateDebTorsListTable(ctx, docFullPath);
                PopulateSuspenseTable(ctx, docFullPath, oledbConnectionString);
            }
        }

        #region [ Private Helpers ]

        private void PopulateSuspenseTable(VTTestContext ctx, string docFullPath, string oledbConectionString)
        {
            using (ExcelHelper excelHelper = new ExcelHelper(docFullPath, true))
            {

                excelHelper.GetResultsFromDocument("Select * From [Suspense$]");

                foreach (ResultRow row in excelHelper.Results)
                {
                    Suspense suspense = new Suspense();
                    foreach (var column in row.Columns)
                    {
                        switch (column.Key.Trim())
                        {
                            case "INTRODUCER (IF KNOWN)":
                                suspense.Introducer = column.Value;
                                break;
                            case "DATE":
                                suspense.Date = column.Value;
                                break;
                            case "METHOD":
                                suspense.Method = column.Value;
                                break;
                            case "AMOUNT":
                                suspense.Amount = column.Value;
                                break;
                        }
                    }
                    ctx.Suspense.Add(suspense);
                }
            }
            ctx.SaveChanges();
        }

        private void PopulateIntroducerCodesTable(VTTestContext ctx, string docFullPath)
        {
            string oledbConnectionString = ConfigurationHelper.Get(AppSettingsList.oledbConnectionString);
            using (ExcelHelper excelHelper = new ExcelHelper(docFullPath, true))
            {
                excelHelper.GetResultsFromDocument("Select * From [Introducer Codes$]");

                foreach (ResultRow row in excelHelper.Results)
                {
                    IntroducerCodes introducer = new IntroducerCodes();
                    foreach (var column in row.Columns)
                    {
                        switch (column.Key.Trim())
                        {
                            case "AGENT_Code":
                                introducer.AGENT_Code = column.Value;
                                break;
                            case "AGENT_Contact":
                                introducer.AGENT_Contact = column.Value;
                                break;
                            case "AGENT_Name":
                                introducer.AGENT_Name = column.Value;
                                break;
                            case "AGENT_TradingName":
                                introducer.AGENT_TradingName = column.Value;
                                break;
                            case "AGENT_Address":
                                introducer.AGENT_Address = column.Value;
                                break;
                            case "AGENT_Email":
                                introducer.AGENT_Email = column.Value;
                                break;
                        }
                    }
                    ctx.IntroducerCodes.Add(introducer);
                }
            }
            ctx.SaveChanges();
        }
        private void PopulateDebTorsListTable(VTTestContext ctx, string docFullPath)
        {
            string oledbConnectionString = ConfigurationHelper.Get(AppSettingsList.oledbConnectionString);
            using (ExcelHelper excelHelper = new ExcelHelper(docFullPath, true))
            {
                excelHelper.GetResultsFromDocument("Select * From [Debtors List$]");

                foreach (ResultRow row in excelHelper.Results)
                {
                    DebtorsList debtor = new DebtorsList();
                    foreach (var column in row.Columns)
                    {
                        switch (column.Key.Trim())
                        {
                            case "Debtor":
                                debtor.Debtor = column.Value;
                                break;
                            case "Currency":
                                debtor.Currency = column.Value;
                                break;
                            case "Reference":
                                debtor.Reference = column.Value;
                                break;
                            case "Balance":
                                debtor.Balance = column.Value;
                                break;
                            case "Not Due":
                                debtor.NotDue = column.Value;
                                break;
                            case "1-30 days":
                                debtor.OneTo30Days = column.Value;
                                break;
                            case "31-60 days":
                                debtor.ThrityOneTo60Days = column.Value;
                                break;
                            case "61-90 days":
                                debtor.SixtyOneto90Days = column.Value;
                                break;
                            case "91-120 days":
                                debtor.NinetyOneTo120Days = column.Value;
                                break;
                            case "over 120 days":
                                debtor.Over120Days = column.Value;
                                break;
                        }
                    }
                    ctx.DebtorsList.Add(debtor);
                }
            }
            ctx.SaveChanges();
        }

        #endregion
    }
}
