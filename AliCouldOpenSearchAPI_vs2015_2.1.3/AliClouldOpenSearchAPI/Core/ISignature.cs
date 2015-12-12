using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliCloud.com.API.Core
{
    /// <summary>
    /// Parameter signature interface
    /// </summary>
    public interface ISignature
    {
        string SignatureMethod { get; }
        string SignatureVersion { get; }

        string Sign(Dictionary<String, Object> parameters);
    }
}
