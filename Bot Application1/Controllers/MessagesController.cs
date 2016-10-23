using System;
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
            string responseText = "";
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            StateClient stateClient = activity.GetStateClient();

            //BotData conversationData = await stateClient.BotState.GetConversationDataAsync(activity.ChannelId, activity.From.Id);

            //if (activity.Type == "UserAddedToConversation")
            //{
            //    responseText = sendGreetings();
            //    conversationData.SetProperty<bool>("SentGreeting", true);
            //    await stateClient.BotState.SetConversationDataAsync(activity.ChannelId, activity.From.Id, conversationData);

            //}
            //Dice roll functionality
            int diceRollResult = rollDice(activity.Text);
            if(diceRollResult != -1)
            {
                responseText = ($"Your dice throw with a {activity.Text} resulted in a {diceRollResult}");
            } else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                HandleSystemMessage(activity);
                await connector.Conversations.ReplyToActivityAsync(activity);
            } else
            {
                responseText = ("I didn't quite understand you. Would you kindly repeat yourself?");
            }

            Activity reply = activity.CreateReply(responseText);
            await connector.Conversations.ReplyToActivityAsync(reply);
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
                string response;
                bool botWasAdded = false;
                //Test if the bot itself was added to the conversation
                for (int i = 0; i < message.MembersAdded.Count; i++)
                {
                    if (message.MembersAdded[i].Id == message.Recipient.Id)
                    {
                        botWasAdded = true;
                    }
                }
                //Different messages, depending on who was added or if someone left.
                if (message.MembersAdded.Count > 0 && botWasAdded == false)
                {
                    response = "The party meets a hobo on the side of the street. Magnanimous as they are, they let him join their group. The party has a new member!";
                } else if (message.MembersAdded.Count > 0 && botWasAdded == true)
                {
                    response = sendGreetings();
                } else
                {
                    response = "The party suffers a heavy blow and loses a member. He will be missed. Probably.";
                }
                    return message.CreateReply(response);
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
                case "d4":
                    return rollDice(4);
                    break;
                case "D6":
                case "d6":
                    return rollDice(6);
                    break;
                case "D8":
                case "d8":
                    return rollDice(8);
                    break;
                case "D10":
                case "d10":
                    return rollDice(10);
                    break;
                case "D12":
                case "d12":
                    return rollDice(12);
                    break;
                case "D20":
                case "d20":
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


        private string sendGreetings()
        {
            String greetings = "Greetings to you, adventurers. I will be your Game Master today. If you want me to roll a dice for you, simpley tell me which one. I have a 'D4', a 'D6', a 'D8', a 'D10', a 'D12' and a 'D20'.";
            return greetings;
        }
    }
}