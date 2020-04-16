using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace AJG.VirtualTrainer.Helper.SharePointHelper
{
    public class SharePointHelper
    {
        private string GetFileByNameCamlQuery = "<View><Query><Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>{0}</Value></Eq></Where></Query></View>";
        private string camlQuery_GetItemByID = "<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>{0}</Value></Eq></Where></Query></View>";
        private string clientContextWebFullUrl = string.Empty;
        private string webRelativeUrl = string.Empty;

        #region [ Constructor ]

        public SharePointHelper(string webFullUrl)
        {
            clientContextWebFullUrl = webFullUrl;
            GetWebServerRelativeURL();
        }

        #endregion

        #region [ Public Methods ]

        public bool SaveDocument(string fileFullPath, string docLibraryName, string fileName, Dictionary<string, object> columnNameValues, out string documentSPServerRelativePath, out string errorMessage)
        {
            documentSPServerRelativePath = string.Empty;
            errorMessage = string.Empty;
            try
            {
                using (ClientContext clientContext = new ClientContext(this.clientContextWebFullUrl))
                {
                    // Save the file.
                    using (Stream filestream = System.IO.File.OpenRead(fileFullPath))
                    {
                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, string.Format("{0}/{1}/{2}", this.webRelativeUrl.TrimEnd('/'), docLibraryName, fileName), filestream, true);
                    }

                    // Get the document libary/list
                    List list = GetSPLists(clientContext, docLibraryName).FirstOrDefault();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = string.Format(GetFileByNameCamlQuery, fileName);

                    // Now update any properties
                    UpdateDocumentFields(list, clientContext, columnNameValues, query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public bool UpdateSPItemById(string docLibraryName, int itemId, Dictionary<string, object> columnNameValues, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (ClientContext clientContext = new ClientContext(this.clientContextWebFullUrl))
                {
                    // get this list
                    List list = GetSPLists(clientContext, docLibraryName).FirstOrDefault();

                    // Now update the list item
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = string.Format(camlQuery_GetItemByID, itemId);

                    // Now update any properties
                    UpdateDocumentFields(list, clientContext, columnNameValues, query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        #endregion

        #region [ Private Methods ]

        private IEnumerable<List> GetSPLists(ClientContext clientContext, string libraryTitle)
        {
            var query = from list
                        in clientContext.Web.Lists
                        where list.Title == libraryTitle
                        select list;

            IEnumerable<List> Lists = clientContext.LoadQuery(query);
            clientContext.ExecuteQuery();

            return Lists;
        }
        private void UpdateDocumentFields(List spList, ClientContext clientContext, Dictionary<string, object> columns, CamlQuery camlQuery)
        {
            // Update the Title and EmailInceptionCode fields for the document.
            ListItemCollection listItems = GetListItems(spList, clientContext, camlQuery);

            foreach (ListItem item in listItems)
            {
                foreach (var column in columns)
                {
                    if (column.Value != null)
                    {
                        Type coulmnValueType = column.Value.GetType();
                        if (coulmnValueType == typeof(System.String) || coulmnValueType == typeof(System.Int32) || coulmnValueType == typeof(System.Double))
                        {
                            item[column.Key] = column.Value.ToString();
                        }
                        else if (coulmnValueType == typeof(System.DateTime))
                        {
                            item[column.Key] = (DateTime)column.Value;
                        }
                        else if (coulmnValueType == typeof(System.Boolean))
                        {
                            item[column.Key] = column.Value.ToString().ToLower() == "true" ? true : false;
                        }
                    }
                }
                item.Update();
                clientContext.ExecuteQuery();
            }
        }
        //private void UpdateDocumentFields(List spList, ClientContext clientContext, Dictionary<string, string> columns, CamlQuery camlQuery)
        //{
        //    // Update the Title and EmailInceptionCode fields for the document.
        //    ListItemCollection listItems = GetListItems(spList, clientContext, camlQuery);
        //    foreach (ListItem item in listItems)
        //    {
        //        foreach (var column in columns)
        //        {
        //            item[column.Key] = column.Value;
        //        }
        //        item.Update();
        //        clientContext.ExecuteQuery();
        //    }
        //}
        private ListItemCollection GetListItems(List docLibrary, ClientContext cc, CamlQuery query)
        {
            ListItemCollection listItems = docLibrary.GetItems(query);
            cc.Load(docLibrary);
            cc.Load(listItems, items => items.IncludeWithDefaultProperties(item => item.ParentList));
            try
            {
                cc.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listItems;
        }
        private string GetWebServerRelativeURL()
        {
            if (string.IsNullOrEmpty(webRelativeUrl))
            {
                using (ClientContext clientContext = new ClientContext(this.clientContextWebFullUrl))
                {
                    clientContext.Load(clientContext.Web, web => web.ServerRelativeUrl);
                    try
                    {
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    webRelativeUrl = clientContext.Web.ServerRelativeUrl;
                }
            }
            return webRelativeUrl;
        }

        #endregion
    }
}
