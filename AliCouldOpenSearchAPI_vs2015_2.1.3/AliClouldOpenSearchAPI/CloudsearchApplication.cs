using System;
using System.Collections.Generic;
using AliCloudOpenSearch.com.API.Modal;

namespace AliCloudOpenSearch.com.API
{
    /// <summary>
    ///     Provides application management related functions
    /// </summary>
    public class CloudsearchApplication
    {
        private readonly CloudsearchApi client;
        private readonly string path;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="client">cloudSearchApi instance</param>
        public CloudsearchApplication(CloudsearchApi client)
        {
            this.client = client;
            path = "/index";
        }


        /// <summary>
        ///     Create an application by specified template
        /// </summary>
        /// <param name="applicationName">application name</param>
        /// <param name="templateName">template name</param>
        /// <returns>response</returns>
        public string CreateByTemplate(string applicationName, string templateName)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("action", "create");
            //parameters.Add("index_name", this.indexName);
            parameters.Add("template", templateName);

            return client.ApiCall(path + "/" + applicationName, parameters);
        }

        /// <summary>
        ///     Remove an application
        /// </summary>
        /// <returns>raw response</returns>
        public string Delete(string applicationName)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("action", "delete");
            return client.ApiCall(path + "/" + applicationName, parameters);
        }

        /// <summary>
        ///     View application status
        /// </summary>
        /// <param name="applicationName">applicatoin name</param>
        /// <returns>raw response</returns>
        public string Status(string applicationName)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("action", "status");
            return client.ApiCall(path + "/" + applicationName, parameters);
        }


        /// <summary>
        ///     List all applications
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="pageSize">page size</param>
        /// <returns>response</returns>
        public string ListApplications(int page = 1, int pageSize = 10)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("page", page.ToString());
            parameters.Add("page_size", pageSize.ToString());

            return client.ApiCall(path, parameters);
        }

        /// <summary>
        ///     Get the application error message from server
        /// </summary>
        /// <param name="applicationName">application name</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">page size</param>
        /// <returns>response</returns>
        public string GetErrorMessage(string applicationName, int page = 1, int pageSize = 10,
            SortMode sortMode = SortMode.DESC)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("page", page.ToString());
            parameters.Add("page_size", pageSize.ToString());
            parameters.Add("sort_mode", Enum.GetName(typeof (SortMode), sortMode));

            return client.ApiCall("/index/error/" + applicationName, parameters);
        }

        /// <summary>
        ///     Rebuild the index
        /// </summary>
        /// <returns></returns>
        public string RebuildIndex(string applicatioName, string operate, string table_name)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(applicatioName), "applicatioName cannot be null or empty");

            var parameters = new Dictionary<string, object>();
            parameters.Add("action", "createtask");

            if (!string.IsNullOrEmpty(operate) && operate == "import")
            {
                parameters.Add("operate", operate);
            }

            if (!string.IsNullOrEmpty(table_name))
            {
                parameters.Add("table_name", table_name);
            }

            return client.ApiCall(path + "/" + applicatioName, parameters);
        }
    }
}