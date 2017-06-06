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
        private int scoreLimit = 15;
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

        private bool enableJackOfDiamond = true;


        public Game(Player[] players)
        {
            this.players = players;
            turnCount = cardset.cardCount / players.Length;

            // subscribing players to status listener events
            foreach (Player player in players)
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
                UpdateScores();
                ShowScores();

                if (ScoreLimitReached())
                {
                    gameOver = true;
                }
                else
                {
                    roundCount++;
                    currentLeader = ++currentLeader % players.Length;
                }
            }

            GameOver();
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

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsStartingPlayer(startSuit, startNumber))
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

            for (int i = 0; i < players.Length; i++)
            {
                players[i].ClearTricks();
            }
        }

        private void PassCards()
        {
            List<HashSet<Card>> cardsList = new List<HashSet<Card>>();

            for (int i = 0; i < players.Length; i++)
            {
                cardsList.Add(players[i].GetCardsToPass());
            }

            // pass cards to left
            if (roundCount % 4 == 0)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    players[(players.Length - i - 1) % players.Length].ReceivePassedCards(cardsList[i]);
                }
            }
            // pass cards to right
            else if (roundCount % 4 == 1)
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

        private void UpdateScores()
        {
            for (int i = 0; i < players.Length; i++)
            {
                int score = 0;

                foreach (Hand trick in players[i].tricks)
                {
                    score += trick.GetCardsOfSuit(CardInfo.GetSuit("Hearts")).Count;

                    if (trick.Contains(CardInfo.GetSuit("Spades"), CardInfo.GetNumber("Queen")))
                    {
                        score += 12;
                    }

                    if (enableJackOfDiamond)
                    {
                        if (trick.Contains(CardInfo.GetSuit("Diamonds"), CardInfo.GetNumber("Jack")))
                        {
                            score -= 10;
                        }
                    }
                }

                players[i].score += score;
            }
        }

        private void ShowScores()
        {
            Console.WriteLine("\n[ SCORES ]\n");

            foreach (Player player in players)
            {
                Console.WriteLine(string.Format("{0}\t\t{1}", player.name, player.score).ToUpper());
            }

            Console.WriteLine();
        }

        private bool ScoreLimitReached()
        {
            foreach(Player player in players)
            {
                if(player.score >= scoreLimit)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles game over activities like displaying the winner.
        /// </summary>
        private void GameOver()
        {
            List<Player> sortedPlayers = new List<Player>(players);
            sortedPlayers.Sort();

            int winningScore = sortedPlayers[0].score;
            List<Player> winningScorers = new List<Player>();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                if (sortedPlayers[i].score > winningScore)
                {
                    break;
                }
                else if (sortedPlayers[i].score == winningScore)
                {
                    winningScorers.Add(sortedPlayers[i]);
                }
            }

            Console.WriteLine("\n[ GAME OVER ]\n");

            if (winningScorers.Count > 1)
            {
                Console.WriteLine("\n[ NO SINGLE WINNER ]\n");
            }
            else
            {
                Console.WriteLine("\n[ {0} HAS WON THE GAME ]\n", winningScorers[0].name.ToUpper());
            }
        }
    }
}