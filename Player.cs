﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Player : IComparable<Player>
    {
        //private User user;
        public string name { get; private set; }
        public int score { get; set; } = 0;
        private Hand hand;
        public List<Hand> tricks { get; private set; } = new List<Hand>();
        private bool heartsBreak = false;
        private bool firstPass = true;
        private bool firstMove = true;


        public Player() { }

        //public Player(User user)
        //{
        //    this.user = user;
        //}

        public Player(string name)
        {
            this.name = name;
        }

        public void AssignHand(Hand hand)
        {
            this.hand = hand;
        }

        public Card PlayCard(int playingSuit = -1)
        {
            int suit, number;
            string strSuit, strNumber;

            Hand playableHand = GetPlayableHand(playingSuit);

            while (true)
            {
                ShowHand(playableHand);
                Console.Write("Select a card ( Suit, Number ): ");

                //bool invalidCard = false;
                //bool invalidSuit = false;
                //bool cardNotFound = false;
                //bool beginningPlayer = false;

                string[] inputs = Console.ReadLine().Split(' ');
                strSuit = inputs[0];
                strNumber = inputs[1];
                suit = CardInfo.GetSuit(strSuit);
                number = CardInfo.GetNumber(strNumber);

                if(playableHand.Contains(suit, number))
                {
                    return hand.PullCard(suit, number);
                }
                //Card card = hand.PullCard(suit, number);


                //if (playingSuit == -1)
                //{
                //    beginningPlayer = true;
                //}
                //if (suit != playingSuit && hand.GetCardsOfSuit(playingSuit).Count > 0)
                //{
                //    invalidSuit = true;
                //}
                //if (suit == -1 && number == -1)
                //{
                //    invalidCard = true;
                //}
                //if (card == null)
                //{
                //    cardNotFound = true;
                //}

                //if (!invalidCard && !cardNotFound && !invalidSuit)
                //{
                //    if (firstMove)
                //    {
                //        if (card.actualSuit.Equals("Clubs") && card.actualNumber.Equals("2"))
                //        {
                //            return card;
                //        }
                //    }
                //    else if (firstPass)
                //    {
                //        if (!(card.actualSuit.Equals("Hearts") || (card.actualSuit.Equals("Spades") && card.actualNumber.Equals("Queen"))))
                //        {
                //            return card;
                //        }
                //    }
                //    else
                //    {
                //        if (card.actualSuit.Equals("Hearts") && !heartsBreak && beginningPlayer) { }
                //        else
                //        {
                //            return card;
                //        }
                //    }
                //}

                // if the card can not be returned then push the card back 
                //hand.PushCard(card);
            }
        }

        public Hand GetPlayableHand(int playingSuit = -1)
        {
            Hand playableHand = new Hand(hand);

            bool beginningPlayer = false;

            if (playingSuit == -1)
            {
                beginningPlayer = true;
            }


            if (firstMove)
            {
                playableHand.Clear();
                playableHand.PushCard(hand.GetCard(CardInfo.GetSuit("Clubs"), CardInfo.GetNumber("2")));
            }
            else
            {
                if (hand.ContainsSuit(playingSuit))
                {
                    playableHand.Clear();
                    playableHand.PushCard(hand.GetCardsOfSuit(playingSuit));
                }
                else
                {
                    if (firstPass)
                    {
                        playableHand.RemoveCardsOfSuit(CardInfo.GetSuit("Hearts"));
                        playableHand.PullCard(CardInfo.GetSuit("Spades"), CardInfo.GetNumber("Queen"));
                    }
                    else if (!heartsBreak && beginningPlayer)
                    {
                        playableHand.RemoveCardsOfSuit(CardInfo.GetSuit("Hearts"));
                    }
                }
            }

            return playableHand;
        }

        public void ShowHand(int suit = -1)
        {
            Console.WriteLine("\n[ {0}'s HAND ]", name.ToUpper());

            if (suit < 0 || suit >= CardInfo.suits.Length || !hand.ContainsSuit(suit))
            {
                Console.WriteLine(hand);
            }
            else
            {
                Console.WriteLine(new Hand(hand.GetCardsOfSuit(suit)));
            }

            Console.WriteLine();
        }

        public void ShowHand(Hand hand)
        {
            Console.WriteLine("\n[ {0}'s HAND ]", name.ToUpper());
            Console.WriteLine(hand);
            Console.WriteLine();
        }

        public override string ToString()
        {
            return name;
        }

        public bool IsStartingPlayer(string suit, string number)
        {
            if (hand.Contains(suit, number))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReceiveTrick(HashSet<Card> cards)
        {
            tricks.Add(new Hand(cards));
        }

        public void ClearTricks()
        {
            tricks.Clear();
        }

        public HashSet<Card> GetCardsToPass()
        {
            HashSet<Card> cards = new HashSet<Card>();
            int cardCount = 3;

            ShowHand();

            Console.WriteLine("\n[ SELECT CARDS TO PASS ]\n");

            while (cards.Count < cardCount)
            {
                cards.Add(PlayCard());
            }

            return cards;
        }

        public void ReceivePassedCards(HashSet<Card> cards)
        {
            foreach (Card card in cards)
            {
                hand.PushCard(card);
            }
        }

        public void HeartsBreakStatusChanged(bool status)
        {
            heartsBreak = status;
        }

        public void FirstPassStatusChanged(bool status)
        {
            firstPass = status;
        }

        public void FirstMoveStatusChanged(bool status)
        {
            firstMove = status;
        }

        public int CompareTo(Player p)
        {
            return score.CompareTo(p.score);
        }
    }
}
