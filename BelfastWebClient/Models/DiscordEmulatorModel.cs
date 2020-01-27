using System.Collections.Generic;
using Discord;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BelfastWebClient.Models
{
    public class DiscordEmulatorModel
    {
        public IUserMessage[] Messages;
    }
}