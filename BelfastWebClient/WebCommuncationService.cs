using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using BelfastBot.Services.Communiciation;
using System.Collections.Concurrent;
using Moq;
using System;

namespace BelfastWebClient
{
    public class WebCommunicationService : ICommunicationService
    {
        public ConcurrentDictionary<ulong, ConcurrentBag<IUserMessage>> Channels = new ConcurrentDictionary<ulong, ConcurrentBag<IUserMessage>>();

        public async Task<IUserMessage> SendMessageAsync(IMessageChannel channel, string message = null, Embed embed = null)
        {
            IUserMessage userMessage = CreateMessage(message, embed);
            Channels.GetOrAdd(channel.Id, new ConcurrentBag<IUserMessage>()).Add(userMessage);
            return userMessage;
        }

        public IUserMessage CreateMessage(string message = null, Embed embed = null)
        {
            Mock<IUserMessage> messageMock = new Mock<IUserMessage>();
            messageMock.Setup(o => o.Content).Returns(message);
            List<Embed> embeds = new List<Embed>();
            if(embed != null)
                embeds.Add(embed);
            messageMock.Setup(o => o.Embeds).Returns(embeds);
            return messageMock.Object;
        }

        public IMessageChannel CreateChannelWithId(ulong id)
        {
            Mock<IMessageChannel> channelMock = new Mock<IMessageChannel>();
            channelMock.Setup(o => o.Id).Returns(0);
            channelMock
                .Setup(o => o.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()))
                .Returns(
                    (string text, bool isTTS, Embed embed, RequestOptions options) 
                    => SendMessageAsync(channelMock.Object, text, embed)
                );
            return channelMock.Object;
        }
    }
}