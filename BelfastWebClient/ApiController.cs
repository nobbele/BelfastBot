using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BelfastBot.Services.Commands;
using Discord;

namespace BelfastWebClient
{
    public class ApiController : Controller
    {
        private ICommandHandlingService _command;
        public WebCommunicationService _communication;

        public ApiController(ICommandHandlingService command, WebCommunicationService communication)
        {
            _command = command;
            _communication = communication;
        }

        [Route("api/botcommand")]
        public async Task<IActionResult> BotCommandAsync(string command) 
        {
            IMessageChannel channel = _communication.CreateChannel(0);
            IUserMessage message = await _communication.SendMessageAsync(channel, _communication.CreateUser("Guest", 2), command);

            if (command == "clear")
                _communication.Channels[channel.Id].Clear();
            else
                await _command.HandleCommandAsync(message, false);

            return Json(new { 
                result = _communication.Channels[0].Select(msg => new {
                    username = msg.Author.Username,
                    userId = msg.Author.Id,
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