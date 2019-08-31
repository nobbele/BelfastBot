using System;

namespace SenkoSanBot.Modules.Gacha
{
    public static class GachaCardExtensionMethods
    {
        public static CardRarity ToPercent(this CardRarity cardRarity, double rarity)
        {
            if (rarity >= 0.6)
                return CardRarity.Bronze;
            else if (rarity >= 0.15)
                return CardRarity.Silver;
            else if (rarity >= 0.01)
                return CardRarity.Gold;
            else
                return CardRarity.Mystic;

            throw new Exception("No such rarity");
        }
    }
}