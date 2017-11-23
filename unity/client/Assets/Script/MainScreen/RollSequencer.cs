using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script
{
    // Client
    // 1) If no data, request data batch BUT NOT UNTIL user has done with last item.
    //    This means spin must be startable without final 9. A way to hack spinning forever
    //    could be repeating the first 3 in the last 3 slots, then restart row while no data.
    // 2) If win, claim immediately. Should probably block, in case of connection errors.
    //    Case: spinning. Enter Nørreport. Win. Claim Win. Error. -> handle this.

    // Server
    // onClientRequestData: create batch, store batch (overwrites previous batch).
    // onClientClaimWin: Verify win possible, award or send error.
    //  1] dont verify at all, 2] verify but don't store, 3] verify and store so claim cannot be repeated.
    //  4] Hybrid, store in a static

    public interface IFinal9Provider
    {
        bool TryClaimFinal9(List<int> row0, List<int> row1, List<int> row2);
    }

    class RollSequencer : IFinal9Provider
    {
        private const int SpinCount0 = 15;
        private const int SpinCount1 = 20;
        private const int SpinCount2 = 25;

        System.Random rnd_ = new System.Random((int)DateTime.UtcNow.Ticks);
        List<IconProbability> distribution_ = new List<IconProbability>();
        List<Roll> currentSequence_ = new List<Roll>();
        int idxNextSequenceIdx_;

        public RollSequencer(MonoBehaviour parent)
        {
            parent.StartCoroutine(UpdateLoopCo());
        }

        IEnumerator UpdateLoopCo()
        {
            while (true)
            {
                bool needDistribution = distribution_.Count == 0;
                while (needDistribution)
                {
                    // Waiting until distribution is being set from outside
                    yield return null;
                    needDistribution = distribution_.Count == 0;
                }

                bool needData = idxNextSequenceIdx_ >= currentSequence_.Count;
                if (needData)
                {
                    yield return GetRollData();
                }

                yield return null;
            }
        }

        IEnumerator GetRollData()
        {
            yield return Server.Instance.GetRollData();
//            currentSequence_ = (List<Roll>)Server.Instance.LastResult;
            object o = Server.Instance.LastResult;
            // TODO: Until the server implements a roll cloudscript we fake it here

            List<Roll> rolls = new List<Roll>();
            for (int i = 0; i < 5; ++i)
            {
                Roll roll = new Roll();
                PotentialWins.Build9(roll.Icons, distribution_);
                rolls.Add(roll);
            }

            currentSequence_ = rolls;
            idxNextSequenceIdx_ = 0;
        }

        public bool TryClaimFinal9(List<int> row0, List<int> row1, List<int> row2)
        {
            bool hasData = idxNextSequenceIdx_ < currentSequence_.Count;
            if (!hasData)
                return false;

            List<int> final9 = currentSequence_[idxNextSequenceIdx_].Icons;
            idxNextSequenceIdx_++;

            row0[0] = final9[0];
            row0[1] = final9[3];
            row0[2] = final9[6];
            row1[0] = final9[1];
            row1[1] = final9[4];
            row1[2] = final9[7];
            row2[0] = final9[2];
            row2[1] = final9[5];
            row2[2] = final9[8];

            return true;
        }

        public void GetRandom(List<int> row0, List<int> row1, List<int> row2)
        {
            FillRowWithRandom(row0, SpinCount0);
            FillRowWithRandom(row1, SpinCount1);
            FillRowWithRandom(row2, SpinCount2);
        }

        private void FillRowWithRandom(List<int> row, int count)
        {
            if (distribution_.Count == 0)
            {
                string error = "Fatal error: distribution is not set";
                Console.WriteLine(error);
                return;
            }

            row.Clear();

            for (int i = 0; i < count; ++i)
            {
                double roll = rnd_.NextDouble();
                int idx = PotentialWins.GetRandomIconIdx(roll, distribution_);
                row.Add(idx);
            }
        }

        public void SetDistribution(List<IconProbability> distribution)
        {
            distribution_ = distribution;

            // Normalize probabilities to a total of 1
            double sum = distribution_.Sum(i => i.P);
            distribution_.ForEach(i => i.P *= 1.0 / sum);

            AppLog.StaticLogInfo("Normalized sum: {0}", distribution_.Sum(i => i.P));
        }
    }
}
