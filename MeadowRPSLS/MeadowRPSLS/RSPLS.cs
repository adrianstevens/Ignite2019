using System;

namespace MeadowRPSLS
{
    public class RPSLS
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

        public Hand Player1 { get; private set; }
        public Hand Player2 { get; private set; }

        Random rand;

        public RPSLS()
        {
            rand = new Random();

            Reset();
        }

        public void Reset()
        {
            Player1 = Hand.none;
            Player2 = Hand.none;
        }

        public void Play()
        {
            Reset();

            Player1 = (Hand)(rand.Next() % 5);
            Player2 = (Hand)(rand.Next() % 5);

            Console.WriteLine($"Player 1 is {Player1}");
            Console.WriteLine($"Player 2 is {Player2}");
        }

        public Result GetResult()
        {
            if (Player1 == Hand.none || Player2 == Hand.none)
            {
                return Result.NoResult;
            }

            if (Player1 == Player2)
            {
                return Result.Tie;
            }

            //basic comparisons
            if (Player1 == Hand.Rock && Player2 == Hand.Paper ||
                Player1 == Hand.Rock && Player2 == Hand.Spock)
            {
                return Result.Player2Wins;
            }

            if (Player1 == Hand.Paper && Player2 == Hand.Scissors ||
                Player1 == Hand.Paper && Player2 == Hand.Lizard)
            {
                return Result.Player2Wins;
            }

            if (Player1 == Hand.Scissors && Player2 == Hand.Rock ||
                Player1 == Hand.Scissors && Player2 == Hand.Spock)
            {
                return Result.Player2Wins;
            }

            if (Player1 == Hand.Lizard && Player2 == Hand.Rock ||
                Player1 == Hand.Lizard && Player2 == Hand.Scissors)
            {
                return Result.Player2Wins;
            }

            if (Player1 == Hand.Spock && Player2 == Hand.Paper ||
                Player1 == Hand.Spock && Player2 == Hand.Lizard)
            {
                return Result.Player2Wins;
            }

            return Result.Player1Wins;
        }
    }
}