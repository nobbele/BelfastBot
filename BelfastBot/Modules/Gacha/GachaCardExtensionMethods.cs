using System;

namespace BelfastBot.Modules.Gacha
{
    public static class GachaCardExtensionMethods
    {
        public static CardRarity ToPercent(this CardRarity cardRarity, double rarity)
        {
            if (rarity >= 0.6)
                return CardRarity.Common;
            else if (rarity >= 0.15)
                return CardRarity.Rare;
            else if (rarity >= 0.01)
                return CardRarity.SR;
            else
                return CardRarity.SSR;

            throw new Exception("No such rarity");
        }
    }
}