using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CERNER_Helix_Workflow_Service
{

    [Serializable]
    [XmlType(TypeName = "SerializableKeyValuePair")]
    public struct SerializableKeyValuePair<K, V>
    {
        public K Key
        { get; set; }

        public V Value
        { get; set; }


        public SerializableKeyValuePair(K k, V v) : this() { Key = k; Value = v; }
    }
}
