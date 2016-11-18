using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;
using System.ServiceModel.Web;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "iEventService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select iEventService.svc or iEventService.svc.cs at the Solution Explorer and start debugging.
    public class iEventService : IEventService
    {
        string strConnectionString = "Endpoint=sb://ea-servicebustest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iHKYjxZ+UeOVO7HXLgkFs0ljSxGiFXEl1GO+0Z56ZXg=";
        string strQueueName = "ea-servicebusqueue";
        string strTopicName = "ea-testTopic";
        string strsubscriptionName0 = "ea-testSubscription3";

        NamespaceManager namespaceManager;

        ///<summary>Create event by passing data by parameters for the topic to apply message to and the massage</summary>
        ///<example>//localhost:52315/EventService.svc/PostEventParam/ea-testTopic/A green door no 48 by param</example>
        ///<param name="msg">The text to be applied to the message property</param>
        ///<param name="topic">The topic on the service bus that this should be sent to</param>
        ///<returns>The ID of the created message on the service bus</returns>
        ///<exception cref="Exception">See response.StatusDescription for error message</exception>
        public string PostEventParam(string topic, string msg)
        {
            try
            {
                _SetUpBus();

                TopicClient Client = TopicClient.CreateFromConnectionString(strConnectionString, topic);
                Random rnd = new Random();
                BrokeredMessage message = new BrokeredMessage(msg);
                message.Properties["messageNumber"] = 1;
                message.Properties["source"] = rnd.Next(1, 4);
                Client.Send(message);
                WebOperationContext ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Created;
                return message.MessageId;
            }
            catch (Exception e)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.StatusDescription = e.Message.ToString();
                return null;
            }
        }
        ///<Summary>Create event by passing data by parameters</Summary>
        ///<example>
        ///localhost:52315/EventService.svc/PostEvent
        ///</example>
        public string PostEvent(EventContent content)
        {
            try
            {
                _SetUpBus();

                TopicClient Client = TopicClient.CreateFromConnectionString(strConnectionString, content.topic);
                Random rnd = new Random();
                BrokeredMessage message = new BrokeredMessage(content.content);
                message.Properties["source"] = rnd.Next(1, 4);
                Client.Send(message);
                WebOperationContext ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Created;
                return message.MessageId;
            }
            catch (Exception e)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.StatusDescription = e.Message.ToString();
                return null;
            }
        }
        /// <summary>
        /// Returns a specific value based on the ID you request
        /// </summary>
        public BusEvent[] GetEvents(string strTopic, string strSub)
        {
            try
            {
                _SetUpBus();

                SubscriptionClient subClient = SubscriptionClient.CreateFromConnectionString(strConnectionString, strTopic, strSub);
                long subClient0MsgCnt = namespaceManager.GetSubscription(strTopic, strSub).MessageCount;

                List<BusEvent> contents = new List<BusEvent>();

                for (long i = 0; i < subClient0MsgCnt; i++)
                {
                    BrokeredMessage recm = null;
                    recm = subClient.Receive();
                    contents.Add(new BusEvent()
                    {
                        source = recm.Properties["source"].ToString(),
                        ID = recm.MessageId,
                        topic = strTopic,
                        msg = recm.GetBody<string>(),
                        queue = strQueueName,
                        subscription = strSub
                    });
                    recm.Complete();
                }
                WebOperationContext ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                return contents.ToArray<BusEvent>();
            }
            catch (Exception e)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.StatusDescription = e.Message.ToString();
                return null;
            }

        }

        private void _SetUpBus()
        {
            namespaceManager = NamespaceManager.CreateFromConnectionString(strConnectionString);

            //check if queue exists, if not create
            if (!namespaceManager.QueueExists(strQueueName))
            {
                namespaceManager.CreateQueue(strQueueName);
            }
            //check if topic exists, if not create
            if (!namespaceManager.TopicExists(strTopicName))
            {
                namespaceManager.CreateTopic(strTopicName);
            }
            //check if subcription1 exists, if not create
            if (!namespaceManager.SubscriptionExists(strTopicName, strsubscriptionName0))
            {
                SqlFilter filter = new SqlFilter("source = 1");
                namespaceManager.CreateSubscription(strTopicName, strsubscriptionName0, filter);
            }
        }
    }
}
