using Intma.OPCDAClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Intma.OpcServiceWeb
{
    [ServiceContract]
    public interface IOpcService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetAllValues")]
        List<OPCObject> GetAllValues();
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetValuesByTags/{tags}")]
        List<OPCObject> GetValuesByTags(string tags);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetValuesByIDS/{ids}")]
        List<OPCObject> GetValuesByIDS(string ids);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetValuesByGroup/{group}")]
        List<OPCObject> GetValuesByGroup(string group);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetEx")]
        string GetEx();
    }
}

