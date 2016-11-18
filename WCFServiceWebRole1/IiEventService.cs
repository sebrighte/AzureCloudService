using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IiEventService" in both code and config file together.
    [ServiceContract]
    public interface IEventService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
        UriTemplate = "GetEvents?topic={strTopic}&sub={strSub}",
        BodyStyle = WebMessageBodyStyle.WrappedResponse,
        ResponseFormat = WebMessageFormat.Json)]

        BusEvent[] GetEvents(string strTopic, string strSub);
        
        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "PostEventParam/{topic}/{msg}",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]

        string PostEventParam(string topic, string msg);
        
        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "PostEvent",
        BodyStyle = WebMessageBodyStyle.WrappedResponse,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]

        string PostEvent(EventContent content);

        //Testing
        //http://eventfacade.cloudapp.net/iEventService.svc/GetEvents?topic=ea-testTopic&sub=ea-testSubscription3
        //http://eventfacade.cloudapp.net/iEventService.svc/PostEventParam/ea-testTopic/A green door no 48 by param
        //http://eventfacade.cloudapp.net/iEventService.svc/PostEvent
        //<EventContent xmlns = "http://www.ukho.gov.uk/schemas/event" >< content > A green door no 48 by XML</content><topic>ea-testTopic</topic></EventContent>
        //{ "topic":"ea-testTopic","content": "A green door no 48 by JSON"}
    }

    [DataContract(Name = "EventContent", Namespace = "http://www.ukho.gov.uk/schemas/event")]
    public class EventContent
    {
        [DataMember(Name = "topic")]
        public string topic { get; set; }
        [DataMember(Name = "content")]
        public string content { get; set; }
    }

    [DataContract(Name = "BusEvent")]
    public class BusEvent
    {
        [DataMember(Name = "source")]
        public string source { get; set; }
        [DataMember(Name = "ID")]
        public string ID { get; set; }
        [DataMember(Name = "msg")]
        public string msg { get; set; }
        [DataMember(Name = "queue")]
        public string queue { get; set; }
        [DataMember(Name = "topic")]
        public string topic { get; set; }
        [DataMember(Name = "subscription")]
        public string subscription { get; set; }
    }
}
