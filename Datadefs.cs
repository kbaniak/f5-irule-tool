/* JSON data maps */
using System;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Datadefs
{
   [DataContract()]
   public class iCRbase
    {
        [DataMember]
        public string link;
        [DataMember]
        public string kind;
        [DataMember]
        public string selfLink;
        [DataMember]
        public int generation;
    }
    [DataContract()]
    public class iCRref
    {
        [DataMember]
        public string link;
    }
    
    [DataContract()]
    public class iCRitem : iCRbase
    {
        [DataMember]
        public string name;
        [DataMember]
        public string partition;
        [DataMember]
        public string fullPath;
        [DataMember]
        public iCRref reference;
        public iCRitem(){}
    }


    [DataContract()]
    public class iCRdevicegroup : iCRbase
    {
        [DataMember]
        public string type;
        [DataMember]
        public string name;
        [DataMember]
        public string NetworkFailover; 
    }

    [DataContract()]
    public class iCRdevice : iCRbase
    {
        [DataMember]
        public string chassisType;
        [DataMember]
        public string timeZone;
        [DataMember]
        public bool selfDevice;
        [DataMember]
        public string name;
        [DataMember]
        public string build;
        [DataMember]
        public string edition;
        [DataMember]
        public string version;
        [DataMember]
        public string failoverState;
        [DataMember]
        public string managementIp;
        [DataMember]
        public string chassisId;
        [DataMember]
        public string hostname;
        [DataMember]
        public string baseMac;
        public iCRdevice(){}
    }

    [DataContract()]
    public class iCRresponse<T> : iCRbase
    {
        public long lastUpdateMicros;
        [DataMember]
        public List<T> items;
        public iCRresponse() { }
    }

    [DataContract()]
    public class iCRPostResponse
    {
        [DataMember]
        public string kind;
        [DataMember]
        public string command;
        [DataMember]
        public string commandResult;
        [DataMember]
        public int code;
        [DataMember]
        public string apiError;
        [DataMember]
        public string message;
        public iCRPostResponse(){}
    }

    [DataContract]
    public class BRule
    {
        [DataMember]
        public int priority;
        public BRule()
        {
            priority = 0;
        }
    }

    [DataContract]
    public class BatchDefinition{
        [DataMember(Name="system-ip")]
        public string target;
        [DataMember]
        public List<String> steps;
        [DataMember]
        public Dictionary<string, BRule> rules;
        [DataMember]
        public Dictionary<string, string> options { get; set; } 
    }

    [DataContract]
    public class iRuleUpload 
    {
        [DataMember]
        public string apiAnonymous;
        [DataMember]
        public string partition;
        [DataMember]
        public string fullPath;
        [DataMember]
        public string kind = "tm:ltm:rule:rulestate";
        [DataMember]
        public string name;
        public iRuleUpload(){ }
    }

    [DataContract]
    public class GenericAnswer : iCRbase
    {
        [DataMember]
        public List< Dictionary<string,iCRbase> > items;

    }
    public class Target
    {
        public iCRdevice device;
        public string hagroup;
        public Target(iCRdevice dev, String hagrp)
        {
            device = dev;
            hagroup = hagrp;
        }
    }
    public class RFRow
    {
        public Dictionary<string,string> columns;
        public RFResult extra;
       public RFRow(){
            extra = null;
            columns = new Dictionary<string,string>();
        } 
        public RFRow(Dictionary<string,string> dict){
            extra = null;
            columns = dict;
        } 
    }
    public class RFResult
    {
        public List<string> header;
        public List<RFRow> nodes;
    }
}