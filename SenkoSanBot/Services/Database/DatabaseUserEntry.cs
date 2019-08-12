using System;
using System.Collections.Generic;
using System.Text;

namespace SenkoSanBot.Services.Database
{
    public class DatabaseUserEntry
    {
        public ulong Id { get; set; }
        public List<Warn> Warns { get; set; }

        public static DatabaseUserEntry CreateNew(ulong userId) => new DatabaseUserEntry
        {
            Id = userId,
            Warns = new List<Warn>(),
        };
    }
}