using System;
using System.Collections.Generic;
using System.Text;

namespace SenkoSanBot.Services.Database
{
    public class DatabaseUserEntry
    {
        public ulong Id { get; set; } = 0;
        public List<Warn> Warns { get; set; } = new List<Warn>();
        public string OsuName { get; set; } = null;
        public int Coin { get; set; } = 100;
        public int GachaRolls { get; set; } = 0;
        public GachaCard[] Cards { get; set; } = null;

        public static DatabaseUserEntry CreateNew(ulong userId) => new DatabaseUserEntry
        {
            Id = userId,
        };
    }
}