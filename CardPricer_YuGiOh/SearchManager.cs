using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
// using package HtmlAgilityPack
// a utility to parse html so I don't need to 
// write 10k lines of code 

namespace CardPricer_YuGiOh
{
    class SearchManager
    {
        private string cardToSearch;
        string u;

        public string Search(int searchType, int saveMode, string[] cards)
        {
            if(cards.Length <= 0)
                return "Error! No cards to search for.";
            
            string displayText = "";

            HtmlWeb web = new HtmlWeb();

            Webbe.Type t = searchType == 0 ? Webbe.Type.CardName : Webbe.Type.Code;

            Task task = Task.Run( async () =>
            {
                foreach (string card in cards)
                {
                    cardToSearch = card;

                    u = Webbe.MakeUrl(card.Trim(), t);

                    HtmlDocument doc = web.Load(u);

                    displayText += GetText(doc, t, saveMode);

                    await Task.Delay(3000);
                }

            });

            task.Wait();

            Console.WriteLine(displayText);

            return displayText;
        }

        string GetText(HtmlDocument document, Webbe.Type type, int saveMode)
        {
            Tuple<string[], string[]> myCards;

            myCards = type == Webbe.Type.CardName ? 
                MethodSite1(document) : MethodSite2(document);

            return saveMode == 0 ? 
                TupleToString(myCards) : TupleToStringExcel(myCards);
        }

        string TupleToString(Tuple<string[],string[]> tuple)
        {
            string text = "";

            int l = tuple.Item1.Length;

            for (int i = 0; i < l; i++)
            {
                text += tuple.Item1[i];
                text += " | ";
                text += tuple.Item2[i];
                text += Environment.NewLine;
            }

            return text;
        }

        string TupleToStringExcel(Tuple<string[], string[]> tuple)
        {
            string text = "";

            int l = tuple.Item1.Length;

            for (int i = 0; i < l; i++)
            {
                text += tuple.Item1[i];
                text += ",";
                text += tuple.Item2[i];
                text += ",";
            }

            return text;
        }

        // Card name search
        private Tuple<string[], string[]> MethodSite1(HtmlDocument document)
        {
            HtmlNode infos = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[4]/div[1]/div[3]");

            List<HtmlNodeCollection> cardRes = new List<HtmlNodeCollection>();

            // Some words can have a "suggestion word" and that adds more 
            // nodes to the page
            int idx = infos.ChildNodes.Count < 6 ? 1 : 3;

            // Trying to get the right div to loop in children
            foreach (HtmlNode div in infos.ChildNodes[idx].ChildNodes)
            {
                if (div.NodeType == HtmlNodeType.Element)
                {
                    if (div.ChildNodes.Count > 0)
                        cardRes.Add(div.ChildNodes);
                }
            }

            int n = cardRes.Count;

            string[] prices = new string[n];
            string[] names = new string[n];

            idx = 0;

            // Writes card name and cost from the cards found by search
            for(int i = 0; i < n; i++)
            {
                HtmlNodeCollection card = cardRes[i];

                string name = card[3].ChildNodes[1].InnerText.Trim();

                if (!name.ToLower().Equals(cardToSearch.ToLower()))
                    continue;

                string cost = card[3].ChildNodes[6].InnerText.Trim();

                cost = cost.Replace("&euro;", "\u20AC");

                prices[idx] = cost;
                names[idx] = name;
                idx++;
            }

            Array.Resize(ref prices, idx);
            Array.Resize(ref names, idx);

            return Tuple.Create(names, prices);
        }

        // Card code search
        private Tuple<string[], string[]> MethodSite2(HtmlDocument document)
        {
            // This xPath gets the item list were are shown cards it's searching for
            // after the ul I've added li to get the first item that the results shows
            string[] prices;
            string[] names;

            HtmlNode cardPath = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[5]/div[1]/div[2]/div[1]/div[1]/div[1]/div[5]/div[2]/ul[1]");
            Console.ReadLine();

            int n = cardPath.ChildNodes.Count;

            if (n <= 0)
                return null;

            prices = new string[n];
            names = new string[n];

            string cardInfosPath;

            for (int i = 0; i < n; i++)
            {
                cardInfosPath = "/li[" + i + "]/div[1]/div[2]";

                HtmlNode cardInfo = cardPath.SelectSingleNode(cardInfosPath);
                HtmlNodeCollection divRes = cardInfo.ChildNodes;

                prices[i] = divRes[5].ChildNodes[1].InnerText.Trim();
                names[i] = divRes[3].InnerText.Trim();
            } 

            return Tuple.Create(names, prices);
        }
    }
}
