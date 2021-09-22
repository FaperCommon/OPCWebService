using Intma.OpcService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using Intma.OPCDAClient;
using System.Text.Json;

namespace Intma.OpcServiceWeb
{
    public class OpcService : IOpcService
    {
        private static Config _config;
        private static OPCDataHandler _handler;
        private static System.Diagnostics.EventLog _eventLog;

        public OpcService()
        {
            try
            {
                if (_handler == null)
                {
                    EventLogInit();
                    ReConfig();
                }
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"Constructor exception: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public List<OPCObject> GetAllValues()
        {
            List<OPCObject> output = new List<OPCObject>();
            try
            {
                output = _handler.OPClist.Values.ToList();
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"GetAllValues ex: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return output;
        }

        public List<OPCObject> GetValuesByTags(string tags)
        {
            List<OPCObject> output = new List<OPCObject>();
            try
            {
                output = _handler.OPClist.Values.Where(a => tags.Contains(a.TagName)).ToList();
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"GetValuesByTags ex: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return output;
        }

        public List<OPCObject> GetValuesByGroup(string group)
        {
            List<OPCObject> output = new List<OPCObject>();
            try
            {
                output = _handler.OPClist.Values.Where(a => a.Groups.Any(b => b == group)).ToList();
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"GetValuesByGroup ex: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
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
                _eventLog.WriteEntry($"GetValuesByIDS ex: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return output;
        }

        private void EventLogInit()
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("IntmaOpcServiceWeb"))
                    System.Diagnostics.EventLog.CreateEventSource("IntmaOpcServiceWeb", "IntmaOpcServiceWeb_EventLog");

                if (_eventLog == null) { 
                    _eventLog = new System.Diagnostics.EventLog()
                    {
                        Log = "IntmaOpcServiceWeb_EventLog",
                        Source = "IntmaOpcServiceWeb"
                    };
                }
            }
            catch
            {
            }
        }

        public string ReConfig()
        {
            try
            {
                if (_handler != null)
                {
                    _handler.Dispose();
                }
                var filepath = System.Configuration.ConfigurationManager.AppSettings["configPath"];
                _config = new Config(filepath);
                _handler = new OPCDataHandler(_config.Server);
                _handler.ScanPeriodFromMilliseconds = _config.UpdateRate;
                foreach (var group in _config.Groups)
                {
                    _handler.AddItems(group.Tags);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"ReConfig ex: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return "Failed to reconfigurate: " + ex.Message;
            }
        }

        public void Update(List<OPCObject> items)
        {
            try
            {
                List<OPCObject> opcItems = new List<OPCObject>();
                foreach(var item in items)
                {
                    var obj = new OPCObject();
                    obj.ID = _handler.OPClist.Values.First(a => a.TagName == item.TagName).ID;
                    obj.Value = item.Value;
                    opcItems.Add(obj);
                }
                _handler.WriteValue(opcItems.ToArray());
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"Update ex: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public OPCObject UpdateValues(string itemsString)
        {
            try
            {
                itemsString = itemsString.Replace(';', ':');
                //_eventLog.WriteEntry("Incoming string: " + itemsString);

                var items = new List<OPCObject>();
                items = JsonSerializer.Deserialize<List<OPCObject>>(itemsString.Replace('\'', '\"'));

                items.ForEach(item =>
                {
                    if (float.TryParse(item.Value.ToString().Replace('.', ','), out float temp))
                    {
                        item.Value = new double();
                        item.Value = temp;
                    }
                    else item.Value = null;
                });

                List<OPCObject> opcItems = new List<OPCObject>();
                foreach (var item in items)
                {
                    //_eventLog.WriteEntry("Item TagName: " + item.TagName + "\nItem Value: " + item.Value + "\nID: " + _handler.OPClist.Values.First(a => a.TagName == item.TagName).ID);
                    opcItems.Add(new OPCObject
                    {
                        ID = _handler.OPClist.Values.First(a => a.TagName == item.TagName).ID,
                        Value = item.Value
                    });
                }

                _handler.WriteValue(opcItems.ToArray());
                return opcItems[0];
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"Update values exception: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return null;// "Failed to Update: " + ex.Message + "\nFormat of query string: [{'TagName';'your_tagname','Value';'your_value'}]";
            }
        }

    }
}
