using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Game
    {
        private CardSet cardset = new CardSet();
        private Table playingTable = new Table();
        private bool gameOver = false;
        private Player[] players;
        private int roundCount = 1;
        private int turnCount;
        private Random rndGen = new Random((int)DateTime.Now.Ticks);


        public Game(Player[] players)
        {
            this.players = players;
            turnCount = cardset.cardCount / players.Length;
        }

        // Game Starts Here
        public void Play()
        {
            int currentLeader = rndGen.Next(0, players.Length - 1);

            while (!gameOver)
            {
                Console.WriteLine("\n[ ROUND {0} ]\n", roundCount);

                ShuffleCards();
                DealCards(currentLeader);
                int currentPlayerIndex = GetStartingPlayer();
                
                for (int turns = 0; turns < turnCount; turns++)
                {
                    InitRound();

                    int currentSuit = -1;
                    
                    for (int i = 0; i < players.Length; i++)
                    {
                        ShowTable();

                        players[currentPlayerIndex].ShowHand(currentSuit);
                        Card playedCard = players[currentPlayerIndex].PlayCard();
                        playingTable.Add(currentPlayerIndex, playedCard);

                        if (i == 0)
                        {
                            currentSuit = playedCard.suit;
                        }

                        currentPlayerIndex = ++currentPlayerIndex % players.Length;
                    }

                    ShowTable();

                    int winner = playingTable.GetWinner();
                    HashSet<Card> cards = playingTable.GetCards();
                    players[winner].ReceiveTrick(cards);
                    currentPlayerIndex = winner;

                    Console.WriteLine((string.Format("\n[ {0} wins the trick ]\n", players[winner]).ToUpper()));
                }

                //Update Score of Each Player
                roundCount++;
            }

            currentLeader = ++currentLeader % players.Length;
        }

        // Deals cards among players
        private void DealCards(int receiverIndex)
        {
            HashSet<Card>[] cardSegments = cardset.GetEqualSegments(players.Length);

            for (int i = 0; i < cardSegments.Length; i++)
            {
                players[receiverIndex].AssignHand(new Hand(cardSegments[i]));
                receiverIndex = ++receiverIndex % players.Length;
            }
        }

        private void ShuffleCards()
        {
            int shuffleCount = 9;

            for (int i = 0; i < shuffleCount; i++)
            {
                cardset.RiffleShuffle();
            }
        }

        private void ShowTable()
        {
            Console.WriteLine("\n\n[ CARDS ON THE TABLE ]\n");
            Console.WriteLine(playingTable);
        }

        private int GetStartingPlayer()
        {
            string startSuit = "Clubs";
            string startNumber = "2";

            for(int i=0; i<players.Length; i++)
            {
                if(players[i].IsStartingPlayer(startSuit, startNumber))
                {
                    return i;
                }
            }

            return 0;
        }

        private void InitRound()
        {
            playingTable.Clear();

            for(int i=0; i<players.Length; i++)
            {
                players[i].ClearTricks();
            }
        }
    }
}