using Intma.OPCDAClient;
using Intma.OpcService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;

namespace Intma.OpcServiceWeb
{
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class OpcService : IOpcService
    {
        readonly Config _config;
        private OPCDataHandler _handler;
        private string _exStr = "";

        public OpcService()
        {
            try {
                _config = new Config();
                _handler = new OPCDataHandler(_config.Server);
                foreach (var group in _config.Groups)
                {
                    _handler.AddItems(group.Tags);
                }
            }
            catch (Exception ex)
            {
                _exStr = ex.Message;
            }
        }

        public List<OPCObject> GetAllValues()
        {
            List<OPCObject> output = new List<OPCObject>();
            try
            {
                _handler.Update();
                output = _handler.OPClist.Values.ToList();
            }
            catch (Exception ex)
            {
                output.Add(new OPCObject() { ID = ex.Message });
            }
            return output;
        }

        public List<OPCObject> GetValuesByTags(string tags)
        {
            List<OPCObject> output = new List<OPCObject>();
            try { 
                _handler.Update();
                output = _handler.OPClist.Values.Where(a => tags.Contains(a.TagName)).ToList();
            }
            catch (Exception ex)
            {
                output.Add(new OPCObject() { ID = ex.Message });
            }
            return output;
        }

        public List<OPCObject> GetValuesByGroup(string group)
        {
            List<OPCObject> output = new List<OPCObject>();
            try { 
                _handler.Update();
                output = _handler.OPClist.Values.Where(a => a.Group == group).ToList();
            }
            catch (Exception ex)
            {
                output.Add(new OPCObject() { ID = ex.Message });
            }
            return output;
        }

        public List<OPCObject> GetValuesByIDS(string ids)
        {
            List<OPCObject> output = new List<OPCObject>();
            try
            {
                _handler.Update();
                output = _handler.OPClist.Values.Where(a => ids.Contains(a.ID)).ToList();
            }
            catch (Exception ex)
            {
                output.Add(new OPCObject() { ID = ex.Message });
            }
            return output;
        }

        public string GetEx()
        {
            return _exStr;
        }

    }
}
