using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VsomRemoteSVC
{
    class JsonData
    {
    }

    [DataContract]
    internal class Json_CONNAISSEMENT
    { 
        [DataMember]
        internal string id;

        [DataMember]
        internal string num;

        [DataMember]
        internal string consignee;

        [DataMember]
        internal string adr; 
        
        [DataMember]
        internal string notify;

        [DataMember]
        internal string client;


        [DataMember]
        internal string chassis;
    }

    [DataContract]
    internal class Json_ELEMENT
    { 
     
         [DataMember]
        internal string id;
         [DataMember]
        internal string elt;
         [DataMember]
        internal string lib;
         [DataMember]
        internal string pu;
         [DataMember]
        internal string qte;
         [DataMember]
        internal string unite;
         [DataMember]
        internal string bl;
         [DataMember]
        internal string codetva;
         [DataMember]
        internal string taux;
         [DataMember]
        internal string idfd;
        [DataMember]
        internal string statut;
        [DataMember]
        internal string codeart;
    }
}
