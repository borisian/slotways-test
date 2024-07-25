using System;
using System.Collections.Generic;

class SlotMachine
{
    static void Main()
    {
        // ReelSets
        List<string[]> reelSets = new List<string[]>
        {
            // Band 1
            new string[] {"sym2", "sym7", "sym7", "sym1", "sym1", "sym5", "sym1", "sym4", "sym5", "sym3", "sym2", "sym3", "sym8", "sym4", "sym5", "sym2", "sym8", "sym5", "sym7", "sym2"},
            // Band 2
            new string[] {"sym1", "sym6", "sym7", "sym6", "sym5", "sym5", "sym8", "sym5", "sym5", "sym4", "sym7", "sym2", "sym5", "sym7", "sym1", "sym5", "sym6", "sym8", "sym7", "sym6", "sym3", "sym3", "sym6", "sym7", "sym3"},
            // Band 3
            new string[] {"sym5", "sym2", "sym7", "sym8", "sym3", "sym2", "sym6", "sym2", "sym2", "sym5", "sym3", "sym5", "sym1", "sym6", "sym3", "sym2", "sym4", "sym1", "sym6", "sym8", "sym6", "sym3", "sym4", "sym4", "sym8", "sym1", "sym7", "sym6", "sym1", "sym6"},
            // Band 4
            new string[] {"sym2", "sym6", "sym3", "sym6", "sym8", "sym8", "sym3", "sym6", "sym8", "sym1", "sym5", "sym1", "sym6", "sym3", "sym6", "sym7", "sym2", "sym5", "sym3", "sym6", "sym8", "sym4", "sym1", "sym5", "sym7"},
            // Band 5
            new string[] {"sym7", "sym8", "sym2", "sym3", "sym4", "sym1", "sym3", "sym2", "sym2", "sym4", "sym4", "sym2", "sym6", "sym4", "sym1", "sym6", "sym1", "sym6", "sym4", "sym8"}
        };

        // Paytable
        Dictionary<string, int[]> payTable = new Dictionary<string, int[]>
        {
            { "sym1", new int[] { 1, 2, 3 } },
            { "sym2", new int[] { 1, 2, 3 } },
            { "sym3", new int[] { 1, 2, 5 } },
            { "sym4", new int[] { 2, 5, 10 } },
            { "sym5", new int[] { 5, 10, 15 } },
            { "sym6", new int[] { 5, 10, 15 } },
            { "sym7", new int[] { 5, 10, 20 } },
            { "sym8", new int[] { 10, 20, 50 } }
        };

        // Loop to restart game
        bool playAgain = true;
        while (playAgain)
        {
            // Launch game
            PlaySlotMachine(reelSets, payTable);

            Console.WriteLine("Press a key to play again, or 'q' to exit.");
            var key = Console.ReadKey();
            if (key.KeyChar == 'q' || key.KeyChar == 'Q')
            {
                playAgain = false;
            }

            // Space
            Console.WriteLine();
        }
    }
    static void PlaySlotMachine(List<string[]> reelSets, Dictionary<string, int[]> payTable)
    {
        // Random stop positions
        Random random = new Random();
        int[] stopPositions = new int[5];
        for (int i = 0; i < stopPositions.Length; i++)
        {
            // Takes a random index from each reelset band
            stopPositions[i] = random.Next(reelSets[i].Length);
        }

        // Build
        string[,] screen = new string[3, 5];
        for (int col = 0; col < 5; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                // Calculates which symbol to get on the band using the stop position and the row index to get the next symbols
                int symbolIndex = (stopPositions[col] + row) % reelSets[col].Length;
                // Places the corresponding symbol in the screen
                screen[row, col] = reelSets[col][symbolIndex];
            }
        }

        // Show stop positions and screen
        Console.WriteLine("Stop Positions: " + string.Join(", ", stopPositions));
        Console.WriteLine("Screen:");
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                Console.Write(screen[row, col] + " ");
            }
            Console.WriteLine();
        }


        int totalWins = 0;
        // Store each win details
        List<string> winDetails = new List<string>();

        // To keep track of counted positions, HashSet to be sure it's counted just once
        HashSet<int> countedPositions = new HashSet<int>(); 

        // Check for winning combinations
        for (int col = 0; col < 5; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                // Skip if this position is already counted
                if (countedPositions.Contains(row * 5 + col))
                    continue;

                // Get current symbol
                string symbol = screen[row, col];

                // Init counter
                int count = 1;

                // Lists positions by transforming the 2D array into a linear calculation ex: screen(0, 1) =  0 * 5 + 1 = 1...
                List<int> winningPositions = new List<int> { row * 5 + col };

                // Check adjacent columns for the same symbol
                for (int nextCol = col + 1; nextCol < 5; nextCol++)
                {
                    bool found = false;
                    for (int nextRow = 0; nextRow < 3; nextRow++)
                    {
                        if (screen[nextRow, nextCol] == symbol)
                        {
                            count++;
                            winningPositions.Add(nextRow * 5 + nextCol);
                            found = true;
                            break;
                        }
                    }
                    if (!found) break;
                }

                // Count wins and show it in win details
                if (count >= 3 && payTable.ContainsKey(symbol))
                {
                    // Get the win amount with the symbol and the position in the paytable and add it in win details
                    int winAmount = payTable[symbol][count - 3];
                    totalWins += winAmount;
                    winDetails.Add($"- Ways win {string.Join("-", winningPositions)}, {symbol} x{count}, {winAmount}");

                    // Mark these positions as counted
                    countedPositions.UnionWith(winningPositions); 
                }
            }
        }

        // Show the payout
        Console.WriteLine("Total wins: " + totalWins);
        foreach (var win in winDetails)
        {
            Console.WriteLine(win);
        }
    }
}
