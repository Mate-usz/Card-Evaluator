using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPricer_YuGiOh
{
    public class Webbe
    {
        public enum Type
        {
            Code,
            CardName
        }

        private const string site1 = @"https://www.magicstore.it/ricerca.php?q=";
        private const string site2 = @"https://www.cardgame-club.it/catalogsearch/result/?q=";

        // Website 1 related things... filters the search for YU GI OH cards
        private const string idSearch = @"&id_cat=10";

        public static string MakeUrl(string cardName, Type t)
        {
            string url = null;
            string formattedString = FormatCardName(cardName);


            // Could simply do a switch
            if (t == Type.CardName)
                url = site1 + formattedString + idSearch;
            else if (t == Type.Code)
                url = site2 + formattedString;

            return url;
        }

        private static string FormatCardName(string card)
        {
            string ret = card;

            return ret.Replace(' ', '+'); ;
        }

    }
}
