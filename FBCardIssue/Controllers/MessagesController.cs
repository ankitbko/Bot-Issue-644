using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;

namespace FBCardIssue
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = CreateReceipt(activity);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity CreateReceipt(Activity activity)
        {
            var receipt = activity.CreateReply();
            receipt.Attachments = new List<Attachment>();

            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: "https://abg20160531123425.azurewebsites.net/assets/images/peugeot_107.jpg"));

            ReceiptItem lineItem = new ReceiptItem()
            {
                Title = "Mercedes",
                Image = cardImages[0],
                Price = "USD 60",
                Subtitle = "Size Small",
                Text = "Seats 4 person"
            };
            List<ReceiptItem> receiptList = new List<ReceiptItem>();
            receiptList.Add(lineItem);

            var facts = new List<Fact>();
            facts.Add(new Fact("Pick Location", "London Airport"));
            facts.Add(new Fact("Pickup Time", "25.08.2016"));
            facts.Add(new Fact("Drop Location", "Waterloo station"));
            facts.Add(new Fact("Drop Time", "26.08.2016"));

            ReceiptCard plCard = new ReceiptCard()
            {
                Title = "Car reservation confirmed",
                Items = receiptList,
                Total = "65.25",
                Tax = "5.25",
                Facts = facts
            };

            receipt.Attachments.Add(plCard.ToAttachment());
            return receipt;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}