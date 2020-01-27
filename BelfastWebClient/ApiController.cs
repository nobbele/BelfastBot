using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BelfastBot.Services.Commands;

namespace BelfastWebClient
{
    public class ApiController : Controller
    {
        private WebCommandHandlingService _command;
        public WebCommunicationService _communication;

        public ApiController(WebCommandHandlingService command, WebCommunicationService communication)
        {
            _command = command;
            _communication = communication;
        }

        [Route("api/botcommand")]
        public async Task<IActionResult> BotCommandAsync(string command, string data) 
        {
            string[] args;
            if(data != null)
                args = JArray.Parse(data).Children<JValue>().Select(val => val.ToString()).ToArray();
            else
                args = new string[0];
            await _command.HandleCommandAsync(command, args);
            return Json(new { 
                result = _communication.Channels[0].Select(msg => new {
                    content = msg.Content,
                    embeds = msg.Embeds.Select(embed => new {
                        title = embed.Title,
                        fields = embed.Fields.Select(field => $"{field.Name}: {field.Value}"),
                    }),
                }),
            });
        }
    }
}