using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    class Table
    {
        private List<Tuple<int, Card>> playerCardTuples = new List<Tuple<int, Card>>();


        public Table() { }

        public void Add(int player, Card card)
        {
            playerCardTuples.Add(Tuple.Create(player, card));
        }

        public int GetWinner()
        {
            if(playerCardTuples.Count < 0)
            {
                return -1;
            }

            int winner = playerCardTuples[0].Item1;
            int playingSuit = playerCardTuples[0].Item2.suit;
            int maxNumber = playerCardTuples[0].Item2.number;

            foreach (Tuple<int, Card> playerCardTuple in playerCardTuples)
            {
                if (playerCardTuple.Item2.suit == playingSuit)
                {
                    if (playerCardTuple.Item2.number > maxNumber || playerCardTuple.Item2.number == 0)
                    {
                        // number = 0 means an ace. Change if otherwise
                        if(maxNumber != 0)
                        {
                            winner = playerCardTuple.Item1;
                            maxNumber = playerCardTuple.Item2.number;
                        }
                    }
                }
            }

            return winner;
        }

        public HashSet<Card> GetCards()
        {
            HashSet<Card> cards = new HashSet<Card>();

            foreach(Tuple<int, Card> playerCardTuple in playerCardTuples)
            {
                cards.Add(playerCardTuple.Item2);
            }

            return cards;
        }

        public void Clear()
        {
            playerCardTuples.Clear();
        }

        public override string ToString()
        {
            string str = "";

            foreach(Tuple<int, Card> playerCardTuple in playerCardTuples)
            {
                str += playerCardTuple.Item2.ToString() + " from " + playerCardTuple.Item1 + "\n";
            }

            return str;
        }
    }
}
