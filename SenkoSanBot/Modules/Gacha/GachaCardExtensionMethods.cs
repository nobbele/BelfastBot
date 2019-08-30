using System;

namespace SenkoSanBot.Modules.Gacha
{
    public static class GachaCardExtensionMethods
    {
        public static CardRarity ToPercent(this CardRarity cardRarity, double rarity)
        {
            if (rarity >= 0.30 && rarity <= 1)
                return CardRarity.Bronze;
            if (rarity >= 0.10 && rarity <= 0.29)
                return CardRarity.Silver;
            if (rarity >= 0 && rarity <= 0.09)
                return CardRarity.Gold;

            throw new Exception("No such rarity");
        }
    }
}