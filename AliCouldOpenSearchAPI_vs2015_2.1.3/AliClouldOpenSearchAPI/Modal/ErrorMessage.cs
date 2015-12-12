using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AliCloudOpenSearch.com.API.Modal
{
    /// <summary>
    /// Responsed errorMessage modal
    /// </summary>
    public class ErrorMessage
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}
