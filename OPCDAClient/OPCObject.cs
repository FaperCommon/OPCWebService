using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Intma.OPCDAClient
{
    [DataContract]
    [Serializable]
    public class OPCObject //: IOpcUpdatable
    {
        private string _id;
        private string _timeStamp;
        private object _value;
        private int _quality;
        private string _tagName;

        [DataMember]
        [JsonInclude]
        public string ID { get { return _id; } set { _id = value; } }
        [DataMember]
        [JsonInclude]
        public string TimeStamp { get { return _timeStamp; } set { _timeStamp = value; } }
        [DataMember]
        [JsonInclude]
        public object Value { get { return _value; } set { _value = value; } }
        [DataMember]
        [JsonInclude]
        public int Quality { get { return _quality; } set { _quality = value; } }
        [DataMember]
        [JsonInclude]
        public string TagName { get => _tagName; set => _tagName = value; }
        public List<string> Groups { get; set; }

        public OPCObject()
        {
            Groups = new List<string>();
        }
        public void OPCUpdate(int quality, object value, string timeStamp)
        {
            Quality = quality;

            Value = value;

            TimeStamp = timeStamp;
        }
    }
}
