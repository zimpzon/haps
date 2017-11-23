using System;
using System.Collections.Generic;
using System.Diagnostics;

public class PotentialWins
{
    public enum Difficulty { Any, Level0_Middle_Nudge, Level1_Bottom_Nudge, Level2_Diagonal_Nudge, Level3_Top_Nudge };

    public static Random Rnd = new Random((int)DateTime.UtcNow.Ticks);

    public static void Build9WithGuarenteedMatch(Difficulty difficulty, List<int> final9, List<IconProbability> distribution)
    {
        // Only difficulty 0, 1 and 2 supported. Force to 0 if invalid.
        bool isSupportedDifficulty =
            difficulty == Difficulty.Level0_Middle_Nudge ||
            difficulty == Difficulty.Level1_Bottom_Nudge ||
            difficulty == Difficulty.Level2_Diagonal_Nudge;

        if (!isSupportedDifficulty)
        {
            Debug.WriteLine("Unsupported difficulty, clamping: " + difficulty.ToString());
            difficulty = Difficulty.Level0_Middle_Nudge;
        }

        // Use distribution[0] (lowest xp) for guarenteed matches
        const int ExcludeIdx = 0;
        var exclude = distribution[ExcludeIdx];

        int retries = 50;
        while (retries-- > 0)
        {
            final9.Clear();

            for (int i = 0; i < 9; ++i)
            {
                double roll = Rnd.NextDouble();
                final9.Add(GetRandomIconIdx(roll, distribution, exclude));
            }

            switch (difficulty)
            {
                case Difficulty.Level0_Middle_Nudge:
                    // HNH
                    final9[3] = ExcludeIdx;
                    final9[1] = ExcludeIdx;
                    final9[5] = ExcludeIdx;
                    break;
                case Difficulty.Level1_Bottom_Nudge:
                    // NHH
                    final9[3] = ExcludeIdx;
                    final9[7] = ExcludeIdx;
                    final9[8] = ExcludeIdx;
                    break;
                case Difficulty.Level2_Diagonal_Nudge:
                    // HNH
                    final9[0] = ExcludeIdx;
                    final9[1] = ExcludeIdx;
                    final9[8] = ExcludeIdx;
                    break;
            }

            List<int> lineIfWin = new List<int> { 0, 0, 0 };
            List<int> iconsToBeMatched = new List<int> { 0, 0, 0 };
            int potentialWinCount = CheckForWin(Difficulty.Any, final9, lineIfWin, iconsToBeMatched);
            // There can only be one to not confuse user.
            // NB! When bottom wraps to top it is not possible to create a diagonal win that cannot
            // also be matched in a straight line. So allow 2 wins for diagonal. For tutorial HNH will
            // be used for diagonal.
            int maxPotentialWins = difficulty == Difficulty.Level2_Diagonal_Nudge ? 2 : 1;
            if (potentialWinCount == maxPotentialWins)
                break;
        }
    }

    public static void Build9(List<int> final9, List<IconProbability> distribution)
    {
        final9.Clear();
        for (int i = 0; i < 9; ++i)
        {
            double roll = Rnd.NextDouble();
            final9.Add(GetRandomIconIdx(roll, distribution));
        }
    }

    public static int GetRandomIconIdx(double roll, List<IconProbability> distribution, IconProbability exclude = null)
    {
        double accumulated = 0.0;

        for (int i = 0; i < distribution.Count; ++i)
        {
            var item = distribution[i];
            accumulated += item.P;

            if (exclude != null && exclude.Id == item.Id)
                continue;

            if (roll <= accumulated)
                return i;
        }

        bool lastItemWasExcluded = exclude.Id == distribution[distribution.Count - 1].Id;
        if (lastItemWasExcluded)
        {
            // Return second to last
            return distribution.Count - 2;
        }

        Debug.WriteLine("Roll was not matched in distribution. If distribution is normalized and roll [0..1] this should not happen.");
        return 0; // Default to first one
    }

    // A winning condition is expected as input
    public static void GetHoldFlags(bool[] flags, List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        int winIdx = final9[iconsToBeMatched[0]]; // Any will do, they must be equal

        // Hold only the ones that MUST be held to win. All 3 might already be correct, but we can hold at most 2.
        flags[0] = final9[lineIfWin[0]] == winIdx && final9[GetIdxAbove(lineIfWin[0])] != winIdx;
        flags[1] = final9[lineIfWin[1]] == winIdx && final9[GetIdxAbove(lineIfWin[1])] != winIdx;
        flags[2] = final9[lineIfWin[2]] == winIdx && final9[GetIdxAbove(lineIfWin[2])] != winIdx;
    }

    public static int CheckForWin(Difficulty level, List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        switch (level)
        {
            case Difficulty.Level0_Middle_Nudge:
                return CheckForMiddleNudge(final9, lineIfWin, iconsToBeMatched);

            case Difficulty.Level1_Bottom_Nudge:
                return CheckForBottomNudge(final9, lineIfWin, iconsToBeMatched);

            case Difficulty.Level2_Diagonal_Nudge:
                return
                    CheckForDiagonalNudgeBackslash(final9, lineIfWin, iconsToBeMatched) +
                    CheckForDiagonalNudgeForwardSlash(final9, lineIfWin, iconsToBeMatched);

            case Difficulty.Level3_Top_Nudge:
                return CheckForTopNudge(final9, lineIfWin, iconsToBeMatched);

            case Difficulty.Any:
            default:
                return // Order matters. Last to be checked will be chosen.
                    CheckForTopNudge(final9, lineIfWin, iconsToBeMatched) +
                    CheckForDiagonalNudgeBackslash(final9, lineIfWin, iconsToBeMatched) +
                    CheckForDiagonalNudgeForwardSlash(final9, lineIfWin, iconsToBeMatched) +
                    CheckForBottomNudge(final9, lineIfWin, iconsToBeMatched) +
                    CheckForMiddleNudge(final9, lineIfWin, iconsToBeMatched);
        }
    }

    static List<int> TempLineIfWin = new List<int> { 0, 0, 0 }; // Global to lessen GC

    static int CheckForTopNudge(List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        TempLineIfWin[0] = 0;
        TempLineIfWin[1] = 1;
        TempLineIfWin[2] = 2;

        return CheckGeneric(final9, TempLineIfWin, lineIfWin, iconsToBeMatched);
    }

    static int CheckForDiagonalNudgeForwardSlash(List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        TempLineIfWin[0] = 6;
        TempLineIfWin[1] = 4;
        TempLineIfWin[2] = 2;

        return CheckGeneric(final9, TempLineIfWin, lineIfWin, iconsToBeMatched);
    }

    static int CheckForDiagonalNudgeBackslash(List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        TempLineIfWin[0] = 0;
        TempLineIfWin[1] = 4;
        TempLineIfWin[2] = 8;

        return CheckGeneric(final9, TempLineIfWin, lineIfWin, iconsToBeMatched);
    }

    static int CheckForBottomNudge(List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        TempLineIfWin[0] = 6;
        TempLineIfWin[1] = 7;
        TempLineIfWin[2] = 8;

        return CheckGeneric(final9, TempLineIfWin, lineIfWin, iconsToBeMatched);
    }

    static int CheckForMiddleNudge(List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        TempLineIfWin[0] = 3;
        TempLineIfWin[1] = 4;
        TempLineIfWin[2] = 5;

        return CheckGeneric(final9, TempLineIfWin, lineIfWin, iconsToBeMatched);
    }

    static void CopyLine(List<int> src, List<int> dst)
    {
        dst.Clear();
        dst.AddRange(src);
    }

    static int GetIdxAbove(int idx)
    {
        int result = idx - 3;
        result = result < 0 ? result + 9 : result;
        return result;
    }

    static int CheckGeneric(List<int> final9, List<int> tempLineIfWin, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        int matchCount = CheckGeneric(final9, tempLineIfWin, iconsToBeMatched);
        if (matchCount > 0)
            CopyLine(tempLineIfWin, lineIfWin);

        return matchCount;
    }

    // 0 1 2
    // 3 4 5
    // 6 7 8
    static int CheckGeneric(List<int> final9, List<int> lineIfWin, List<int> iconsToBeMatched)
    {
        int result = 0;

        // Is icon already correctly placed if we hold?
        int h0 = lineIfWin[0];
        int h1 = lineIfWin[1];
        int h2 = lineIfWin[2];

        // Is icon correctly placed if we nudge?
        int n0 = GetIdxAbove(lineIfWin[0]);
        int n1 = GetIdxAbove(lineIfWin[1]);
        int n2 = GetIdxAbove(lineIfWin[2]);

        // HHN
        if (AreEqual(final9, h0, h1, n2, iconsToBeMatched))
            result++;

        // HNH
        if (AreEqual(final9, h0, n1, h2, iconsToBeMatched))
            result++;

        // NHN
        if (AreEqual(final9, n0, h1, n2, iconsToBeMatched))
            result++;

        // NHH
        if (AreEqual(final9, n0, h1, h2, iconsToBeMatched))
            result++;

        // HNN
        if (AreEqual(final9, h0, n1, n2, iconsToBeMatched))
            result++;

        // NNN
        if (AreEqual(final9, n0, n1, n2, iconsToBeMatched))
            result++;

        // NNH
        if (AreEqual(final9, n0, n1, h2, iconsToBeMatched))
            result++;

        return result;
    }

    static bool AreEqual(List<int> final9, int idx0, int idx1, int idx2, List<int> iconsToBeMatched)
    {
        bool areEqual = (final9[idx0] == final9[idx1]) && (final9[idx0] == final9[idx2]);
        if (!areEqual)
            return false;

        iconsToBeMatched[0] = idx0;
        iconsToBeMatched[1] = idx1;
        iconsToBeMatched[2] = idx2;
        return true;
    }
}
