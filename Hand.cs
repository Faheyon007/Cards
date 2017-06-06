using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cards
{
    public class Hand
    {
        private HashSet<Card> cards = new HashSet<Card>();


        public Hand() { }

        public Hand(Card[] cards)
        {
            foreach(Card card in cards)
            {
                this.cards.Add(card);
            }

            Sort();
        }

        public Hand(HashSet<Card> cards)
        {
            foreach (Card card in cards)
            {
                this.cards.Add(card);
            }

            Sort();
        }

        public bool Contains(int suit, int number)
        {
            foreach(Card card in cards)
            {
                if(card.suit == suit && card.number == number)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(string suit, string number)
        {
            foreach (Card card in cards)
            {
                if (card.suit == CardInfo.GetSuit(suit) && card.number == CardInfo.GetNumber(number))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsSuit(int suit)
        {
            if(GetCardsOfSuit(suit).Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool ContainsSuit(string suit)
        {
            if (GetCardsOfSuit(CardInfo.GetSuit(suit)).Count > 0)
            {
                return true;
            }

            return false;
        }

        public Card GetCard(int suit, int number)
        {
            foreach(Card card in cards)
            {
                if(card.suit == suit && card.number == number)
                {
                    return card;
                }
            }

            return null;
        }

        public Card PullCard(int suit, int number)
        {
            foreach (Card card in cards)
            {
                if (card.suit == suit && card.number == number)
                {
                    Card c = card.Clone() as Card;
                    cards.Remove(card);
                    return c;
                }
            }

            return null;
        }

        public void PushCard(Card card)
        {
            if (card != null)
            {
                cards.Add(card);

            }

            Sort();
        }

        public HashSet<Card> GetCardsOfSuit(int suit)
        {
            HashSet<Card> cardsOfSuit = new HashSet<Card>();

            foreach(Card card in cards)
            {
                if(card.suit == suit)
                {
                    cardsOfSuit.Add(card);
                }
            }

            return cardsOfSuit;
        }
        
        public void Sort()
        {
            var sortedCards = cards.ToList();
            sortedCards.Sort();
            cards = new HashSet<Card>(sortedCards);
        }

        public override string ToString()
        {
            string str = "";

            foreach(var card in cards)
            {
                str += card + "\n";
            }

            return str;
        }
    }
}