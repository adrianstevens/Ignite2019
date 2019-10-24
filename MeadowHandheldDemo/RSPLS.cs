using System;
namespace MeadowHandheldDemo
{
    public class RSPLS
    {
        public enum Hand
        {
            Rock,
            Paper,
            Scissors,
            Lizard,
            Spock,
            none
        }

        public enum Result
        {
            Player1Wins,
            Player2Wins,
            Tie,
            NoResult
        }

        Hand player1, player2;

        Random rand;

        public RSPLS()
        {
            rand = new Random();

            Reset();
        }

        public void Reset()
        {
            player1 = Hand.none;
            player2 = Hand.none;
        }

        public void Play()
        {
            Reset();

            player1 = (Hand)(rand.Next() % 5);
            player2 = (Hand)(rand.Next() % 5);
        }

        public Result GetResult()
        {
            if (player1 == Hand.none || player2 == Hand.none)
            {
                return Result.NoResult;
            }

            if (player1 == player2)
            {
                return Result.Tie;
            }

            //logic time

            /*    if (player1 == Hand.Lizard && player2 == Hand.Spock ||
                    player1 == Hand.Lizard && player2 == Hand.Rock ||
                    player1 == Hand.Paper &&

                */
            return Result.Player1Wins;
        }


    }
}
