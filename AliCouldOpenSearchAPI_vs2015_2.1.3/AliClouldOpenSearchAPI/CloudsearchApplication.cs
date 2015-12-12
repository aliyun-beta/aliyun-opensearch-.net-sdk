using AliCloudOpenSearch.com.API.Modal;

namespace AliCloudOpenSearch.com.API
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System.Collections.Specialized;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides application management related functions
    /// </summary>
    public class CloudsearchApplication
    {

        private CloudsearchApi client;
        private String path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">cloudSearchApi instance</param>
        public CloudsearchApplication(CloudsearchApi client)
        {
            this.client = client;
            this.path = "/index";
        }


        /// <summary>
        /// Create an application by specified template
        /// </summary>
        /// <param name="applicationName">application name</param>
        /// <param name="templateName">template name</param>
        /// <returns>response</returns>
        public String CreateByTemplate(string applicationName, string templateName)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("action", "create");
            //parameters.Add("index_name", this.indexName);
            parameters.Add("template", templateName);

            return this.client.ApiCall(this.path + "/" + applicationName, parameters);
        }

        /// <summary>
        /// Remove an application
        /// </summary>
        /// <returns>raw response</returns>
        public String Delete(string applicationName)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("action", "delete");
            return this.client.ApiCall(this.path + "/" + applicationName, parameters);
        }

        /// <summary>
        /// View application status
        /// </summary>
        /// <param name="applicationName">applicatoin name</param>
        /// <returns>raw response</returns>
        public String Status(string applicationName)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("action", "status");
            return this.client.ApiCall(this.path + "/" + applicationName, parameters);
        }


        /// <summary>
        /// List all applications
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="pageSize">page size</param>
        /// <returns>response</returns>
        public String ListApplications(Int32 page = 1, Int32 pageSize = 10)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("page", page.ToString());
            parameters.Add("page_size", pageSize.ToString());

            return this.client.ApiCall(this.path, parameters);
        }

        /// <summary>
        /// Get the application error message from server
        /// </summary>
        /// <param name="applicationName">application name</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">page size</param>
        /// <returns>response</returns>
        public String GetErrorMessage(string applicationName, Int32 page = 1, Int32 pageSize = 10, SortMode sortMode = SortMode.DESC)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("page", page.ToString());
            parameters.Add("page_size", pageSize.ToString());
            parameters.Add("sort_mode", Enum.GetName(typeof(SortMode), sortMode));

            return this.client.ApiCall("/index/error/" + applicationName, parameters);
        }

        /// <summary>
        /// Rebuild the index
        /// </summary>
        /// <returns></returns>
        public string RebuildIndex(string applicatioName, string operate, string table_name)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(applicatioName), "applicatioName cannot be null or empty");

            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("action", "createtask");

            if (!string.IsNullOrEmpty(operate) && operate == "import")
            {
                parameters.Add("operate", operate);
            }

            if (!string.IsNullOrEmpty(table_name))
            {
                parameters.Add("table_name", table_name);
            }

            return this.client.ApiCall(this.path + "/" + applicatioName, parameters);
        }
    }
}
