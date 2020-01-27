using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace IAsyncEnumerableFix
{
    public abstract class UserMessage : IUserMessage
    {
        public virtual IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => throw new NotImplementedException();

        public virtual MessageType Type => throw new NotImplementedException();

        public virtual MessageSource Source => throw new NotImplementedException();

        public virtual bool IsTTS => throw new NotImplementedException();

        public virtual bool IsPinned => throw new NotImplementedException();

        public virtual string Content => throw new NotImplementedException();

        public virtual DateTimeOffset Timestamp => throw new NotImplementedException();

        public virtual DateTimeOffset? EditedTimestamp => throw new NotImplementedException();

        public virtual IMessageChannel Channel => throw new NotImplementedException();

        public virtual IUser Author => throw new NotImplementedException();

        public virtual IReadOnlyCollection<IAttachment> Attachments => throw new NotImplementedException();

        public virtual IReadOnlyCollection<IEmbed> Embeds => throw new NotImplementedException();

        public virtual IReadOnlyCollection<ITag> Tags => throw new NotImplementedException();

        public virtual IReadOnlyCollection<ulong> MentionedChannelIds => throw new NotImplementedException();

        public virtual IReadOnlyCollection<ulong> MentionedRoleIds => throw new NotImplementedException();

        public virtual IReadOnlyCollection<ulong> MentionedUserIds => throw new NotImplementedException();

        public virtual MessageActivity Activity => throw new NotImplementedException();

        public virtual MessageApplication Application => throw new NotImplementedException();

        public virtual DateTimeOffset CreatedAt => throw new NotImplementedException();

        public virtual ulong Id => throw new NotImplementedException();

        public virtual Task AddReactionAsync(IEmote emote, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task PinAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task RemoveAllReactionsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
        {
            throw new NotImplementedException();
        }

        public virtual Task UnpinAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
