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
            string responseText = "";
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            StateClient stateClient = activity.GetStateClient();
            
            //Dice roll functionality
            int diceRollResult = rollDice(activity.Text);
            if(diceRollResult != -1)
            {
                responseText = ($"Your dice throw with a {activity.Text} resulted in a {diceRollResult}");
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                Activity greeting = HandleSystemMessage(activity);
                responseText = greeting.Text;
            }
            else
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
                bool botAddedToConversation = botWasAdded(message);

                //Different messages, depending on who was added or if someone left.
                if (message.MembersAdded.Count > 0 && botAddedToConversation == false)
                {
                    response = "The party meets a hobo on the side of the street. Magnanimous as they are, they let him join their group. The party has a new member!";
                } else if (message.MembersAdded.Count > 0 && botAddedToConversation == true)
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
                case "D6":
                case "d6":
                    return rollDice(6);
                case "D8":
                case "d8":
                    return rollDice(8);
                case "D10":
                case "d10":
                    return rollDice(10);
                case "D12":
                case "d12":
                    return rollDice(12);
                case "D20":
                case "d20":
                    return rollDice(20);
                default:
                    return -1;
            }
        }

        private int rollDice(int numberOfSides)
        {
            Random random = new Random();
            numberOfSides++;
            return random.Next(1, numberOfSides);
        }


        private string sendGreetings()
        {
            String greetings = "Greetings to you, adventurers. I will be your Game Master today. If you want me to roll a dice for you, simpley tell me which one. I have a 'D4', a 'D6', a 'D8', a 'D10', a 'D12' and a 'D20'.";
            return greetings;
        }

        private bool botWasAdded(Activity activity)
        {
            for (int i = 0; i < activity.MembersAdded.Count; i++)
            {
                if (activity.MembersAdded[i].Id == activity.Recipient.Id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}