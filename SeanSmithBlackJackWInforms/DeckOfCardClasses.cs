using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeanSmithBlackJackWInforms
{
    public class Shuffle
    {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public bool shuffled { get; set; }
        public int remaining { get; set; }
    }

    public class Images
    {
        public string svg { get; set; }
        public string png { get; set; }
    }

    public class Card
    {
        public string code { get; set; }
        public string image { get; set; }
        public Images images { get; set; }
        public string value { get; set; }
        public string suit { get; set; }
    }

    public class CardResponse
    {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public IList<Card> cards { get; set; }
        public int remaining { get; set; }
    }

}
