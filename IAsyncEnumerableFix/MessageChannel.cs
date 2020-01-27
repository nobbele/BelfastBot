using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;

namespace IAsyncEnumerableFix
{
    public abstract class MessageChannel : IMessageChannel
    {
        public virtual string Name => throw new NotImplementedException();
        public virtual DateTimeOffset CreatedAt => throw new NotImplementedException();
        public virtual ulong Id => throw new NotImplementedException();

        public virtual Task DeleteMessageAsync(ulong messageId, RequestOptions options = null) => throw new NotImplementedException();
        public virtual Task DeleteMessageAsync(IMessage message, RequestOptions options = null) => throw new NotImplementedException();
        public virtual IDisposable EnterTypingState(RequestOptions options = null) => throw new NotImplementedException();
        public virtual Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) => throw new NotImplementedException();
        public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) => throw new NotImplementedException();
        public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) => throw new NotImplementedException();
        public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) => throw new NotImplementedException();
        public virtual Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null) => throw new NotImplementedException();
        public virtual Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) => throw new NotImplementedException();
        public virtual IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) => throw new NotImplementedException();
        public virtual Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false) => throw new NotImplementedException();
        public virtual Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false) => throw new NotImplementedException();
        public virtual Task<IUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) => throw new NotImplementedException();
        public virtual Task TriggerTypingAsync(RequestOptions options = null) => throw new NotImplementedException();
    }
}