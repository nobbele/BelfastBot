namespace YohaneBot.Services.Database
{
    public struct Warn
    {
        public string Reason;
        public ulong ById;

        public Warn(string reason, ulong byId)
        {
            Reason = reason;
            ById = byId;
        }
    }
}