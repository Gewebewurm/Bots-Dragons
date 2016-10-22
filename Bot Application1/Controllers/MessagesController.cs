﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Bot_Application1
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
            //TODO: Remove unused code after it has served as example for other functions
            //if (activity.Type == ActivityTypes.Message)
            //{
            //    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            //    // calculate something for us to return
            //    int length = (activity.Text ?? string.Empty).Length;

            //    // return our reply to the user
            //    Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
            //    await connector.Conversations.ReplyToActivityAsync(reply);
            //}
            //else
            //{
            //    HandleSystemMessage(activity);
            //}
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            //Dice roll functionality
            int diceRollResult = rollDice(activity.Text);
            if(diceRollResult != -1)
            {
                Activity reply = activity.CreateReply($"Your dice throw with a {activity.Text} resulted in a {diceRollResult}");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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

        private int rollDice(string text)
        {
            string bla = text;
            
            switch (bla)
            {
                case "D4":
                    return rollDice(4);
                    break;
                case "D6":
                    return rollDice(6);
                    break;
                case "D8":
                    return rollDice(8);
                    break;
                case "D10":
                    return rollDice(10);
                    break;
                case "D12":
                    return rollDice(12);
                    break;
                case "D20":
                    return rollDice(20);
                    break;
                default:
                    return -1;
            }
        }

        private int rollDice(int numberOfSides)
        {
            Random random = new Random();
            return random.Next(1, numberOfSides);
        }
    }
}