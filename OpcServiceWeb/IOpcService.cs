using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;

using Intma.OPCDAClient;

namespace Intma.OpcServiceWeb
{
    [ServiceContract]
    public interface IOpcService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetAllValues")]
        List<OPCObject> GetAllValues();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetValuesByTags/{tags}")]
        List<OPCObject> GetValuesByTags(string tags);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetValuesByIDS/{ids}")]
        List<OPCObject> GetValuesByIDS(string ids);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetValuesByGroup/{group}")]
        List<OPCObject> GetValuesByGroup(string group);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ReConfig")]
        string ReConfig();

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Update")]
        void Update(List<OPCObject> items);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "UpdateValues/{items}")]
        OPCObject UpdateValues(string items);
    }
}

