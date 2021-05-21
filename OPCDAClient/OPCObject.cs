using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Intma.OPCDAClient
{
    [DataContract]
    public class OPCObject : IOpcUpdatable
    {
        private string _id;
        private string _timeStamp;
        private object _value;
        private int _quality;
        private string _tagName;
        private string _SVGName;

        [DataMember]
        public string ID { get { return _id; } set { _id = value; } }
        [DataMember]
        public string TimeStamp { get { return _timeStamp; } set { _timeStamp = value; } }
        [DataMember]
        public object Value { get { return _value; } set { _value = value; } }
        [DataMember]
        public int Quality { get { return _quality; } set { _quality = value; } }
        [DataMember]
        public string TagName { get => _tagName; set => _tagName = value; }
        [DataMember]
        public string SVGName { get => _SVGName; set => _SVGName = value; }
        public string Group { get; set; }

        public void OPCUpdate(int quality, object value, string timeStamp)
        {
            Quality = quality;

            Value = value;

            TimeStamp = timeStamp;
        }
    }
}
