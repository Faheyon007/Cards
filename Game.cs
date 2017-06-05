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

        public delegate void HeartsBreakStatusListener(bool status);
        public event HeartsBreakStatusListener heartsBreakStatusListener;
        public delegate void FirstPassStatusListener(bool status);
        public event FirstPassStatusListener firstPassStatusListener;
        public delegate void FirstMoveStatusListener(bool status);
        public event FirstMoveStatusListener firstMoveStatusListener;


        public Game(Player[] players)
        {
            this.players = players;
            turnCount = cardset.cardCount / players.Length;

            // subscribing players to status listener events
            foreach(Player player in players)
            {
                heartsBreakStatusListener += player.HeartsBreakStatusChanged;
                firstPassStatusListener += player.FirstPassStatusChanged;
                firstMoveStatusListener += player.FirstMoveStatusChanged;
            }
        }

        // Game Starts Here
        public void Play()
        {
            int currentLeader = rndGen.Next(0, players.Length - 1);

            while (!gameOver)
            {
                Console.WriteLine("\n[ ROUND {0} ]\n", roundCount);

                InitTurn();
                ShuffleCards();
                DealCards(currentLeader);
                //PassCards();

                int currentPlayerIndex = GetStartingPlayer();

                for (int turns = 0; turns < turnCount; turns++)
                {
                    playingTable.Clear();

                    int currentSuit = -1;
                    
                    for (int i = 0; i < players.Length; i++)
                    {
                        ShowTable();

                        players[currentPlayerIndex].ShowHand(currentSuit);
                        Card playedCard = players[currentPlayerIndex].PlayCard(currentSuit);

                        if (playedCard.actualSuit == "Hearts")
                        {
                            heartsBreakStatusListener(true);
                        }

                        playingTable.Add(currentPlayerIndex, playedCard);

                        if (i == 0)
                        {
                            firstMoveStatusListener(false);
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

                    firstPassStatusListener(false);
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

        private void InitTurn()
        {
            heartsBreakStatusListener(false);
            firstPassStatusListener(true);
            firstMoveStatusListener(true);

            for (int i=0; i<players.Length; i++)
            {
                players[i].ClearTricks();
            }
        }

        private void PassCards()
        {
            List<HashSet<Card>> cardsList = new List<HashSet<Card>>();

            for(int i=0; i<players.Length; i++)
            {
                cardsList.Add(players[i].GetCardsToPass());
            }

            // pass cards to left
            if (roundCount % 4 == 0)
            {
                for(int i=0; i<players.Length; i++)
                {
                    players[(players.Length - i - 1) % players.Length].ReceivePassedCards(cardsList[i]);
                }
            }
            // pass cards to right
            else if(roundCount % 4 == 1)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    players[(i + 1) % players.Length].ReceivePassedCards(cardsList[i]);
                }
            }
            // pass cards to front
            else if (roundCount % 4 == 2)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    players[(i + 2) % players.Length].ReceivePassedCards(cardsList[i]);
                }
            }
        }
    }
}