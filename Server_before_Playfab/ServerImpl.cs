using HapsServerInterface;
using System;
using System.Collections.Generic;

namespace HapsServer
{
    public class ServerImpl : IGameServer
    {
        List<IconProbability> probabilitiesStatic_ = new List<IconProbability>();
        List<IconProbability> probabilitiesDynamic_ = new List<IconProbability>();

        IPersistentStorage persistentStorage_;
        ICacheStorage cacheStorage_;
        Random rnd_ = new Random((int)DateTime.UtcNow.Ticks);

        public ServerImpl(IPersistentStorage persistentStorage, ICacheStorage cacheStorage)
        {
            persistentStorage_ = persistentStorage;
            cacheStorage_ = cacheStorage;

            // What we want to express is chance of 3 of a kind per spin
            // But client wants chance per icon for displaying randoms
            probabilitiesStatic_.Add(new IconProbability() { Id = "C0", P = 0.2f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "C1", P = 0.1f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "C2", P = 0.3f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "C3", P = 0.1f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "C4", P = 0.7f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "C5", P = 0.1f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "TI", P = 0.1f });
            probabilitiesStatic_.Add(new IconProbability() { Id = "JP", P = 0.05f });

            probabilitiesDynamic_.Add(new IconProbability() { Id = "KR", P = 0.01f });
        }

        public void Authenticate(Credentials client, Action<GameServerResult, string> callback)
        {
            callback(GameServerResult.Success, "faketoken");
        }

        public void ClaimWin(Credentials client, List<int> final9, long batchId, Action<GameServerResult> callback)
        {
            callback(GameServerResult.Success);
        }

        public void GetPlayerBaseData(Credentials client, Action<GameServerResult, PlayerBaseData> callback)
        {
            PlayerBaseData data = new PlayerBaseData();
            data.UserId = "000user_id";
            data.DisplayName = "Peter";
            data.CurrentDoneeId = "doneeId";
            data.TitlePreId = "preId";
            data.TitlePostId = "postId";
            data.Xp = 1;

            callback(GameServerResult.Success, data);
        }

        // TODO:
        // Game states sucks atm.
        // 

        //We can look even deeper at why the “Premium Currency” formula works so well.It’s harnessing some
        //well documented psychological patterns with it players. Everybody experienced loss aversion, we
        //are afraid of waste and try hard to avoid it. Premium currency packs take advantage of this loss
        //aversion through the “Sunk Cost” effect to encourage its players to pay more. The “Sunk Cost”
        //effect is  when not losing money in a losing proposition becomes the main reason to throw in more
        //money. In this case leaving unspent Premium Currency is the “losing proposition” and the player,
        //having already invested money into the game, is inclined to spend more to acquire additional
        //premium currency and use the unspent currency sitting in their accounts effectively, thereby in
        //their mind, nullifying the initial loss.

        // Loss/waste aversion makes people return to spend that "whatever". 
        // Win lottery tickets as currency. OMG OMG Offline spin.

        // Pitch
        // Basic idea: money from corps (or players) -> player -> donations
        // Game: Copy popular slot machines (like Slotutopia, 100 mill users)
        // Highly motivated players, greater purpose than the game itself
        // "Spilfonden"

        // As soon as one scenario is completed it is piece of cake to outsource gfx for more

        // Wandering pet er realistisk!!! Kør round-robin på alle pets. Helt ok hvis de står stille et stykke tid.
        // url?pet_id - can show pet name over moving pet icon. User name + title + picture as UI.
        // Pets can report back. Greetings from blahblah. Found stuff. Met other pets.
        // Original, fun, cool, innovative.
        // Need cheaper server.
        // OHH YEEEAH CAAAAN DOOOO!

        // Near misses on KR: When matching 3 KR user has X chance to actually win.
        // Server sends final9 with win + metadata: outcomes = 10, roll = 4, where only 0 wins.
        // - awesome idea: Show the donee. This increases donee exposure to 1 - x.
        // NEAR NEAR MISS - roll one or two icons with no chance of win, but show donee.

        // Gamesparks or Playfab?
        // Spillet hvor alle vinder
        // Vi er ikke som de andre - vi er noget for sig selv - vi spiller spil på nettet, men vinder alligevel

        // Lessons from very popular slot machines (100 mill users):
        // Need credits that you can run out of. Something to lose.
        // Need adjustable betting amount or you will never run out after a big win.
        // A credit jackpot should tick up slowly.
        // Need levels and something to unlock when gaining levels.
        // Very simple mini games. Select a random item, win or lose etc.
        // Multiple ways to win credits. Quests. Collectables, 3-on-a-row, mini-game, jackpot, buffs, level-up, tournaments?
        // -- Missing a coherent credit vs kronor strategy.

        // Is puzzle mode too hard? (gaaaah). Flags:
        //   1) Autospin until possible win
        //   2) Mascot showing 'You can win!'
        //   3) Button to show how (possibly with a timer)

        // Danish youtubers!! omgwtf, they would love to help of course. En sag for Kaktus. Referral codes from youtuber.

        // Metadata on rolls

        // Minimizing number of Playfab calls:
        //  Return a batch of rolls (high priority wins are not batched). Store them in server Player data.
        //  Client saves batch responses locally. Flushed on full, startup, quit, deactivate, timer, etc.
        //  This means wins can be lost if connection dies (Nørreport), app is closed (can't play with no
        //  internet), and later user plays on another device. The wins are orphaned.

        // An easy solution could be... exclude big wins. Fill according to probability for small wins.
        // Add icons for big wins (partial/teasing/etc.). This will skew small wins downwards, but only
        // a little and it doesn't matter much.

        // X0 = 10xp
        // X1 = 50xp
        // X2 = 100xp
        // X3 = 500xp
        // X4 = 1000xp
        // RL = Random letter for word, may not match
        // RP = Random pet
        // KR = 1kr
        // JP = jackpot
        // (more things to fill up)

        // A word, left = level permanent pet when full. (normal and SUPER letters)
        // Permanent pet = xp * 2^level, digs up super letters that always match. One per day. Max one.
        // Other pets: helps digging = subtract from letter timer, +x% xp, other?
        // Tutorial: level 0-x, client only. Force increasingly harder matches. 
        // Need one more thing to fill up on main screen - 
        //
        // level rewards: titles, slight increased money win chance, other?

        // Slotomania:
        // Pick carrots, not moles. Seems pretty fun.
        // Tournaments, whoever wins the most
        // New skins, pretty much
        // Limited credits?

        // Lets sponsor logos/donating player names move across the screen now and then, nonintrusive

        // -- Summary --
        // Credits
        // Adjustable bet (maybe auto)
        // Lottery ticket for offline spinning every x seconds. Can group wins when displaying.
        //    Win: Credits, tickets, letters?, kroner? Always a few tickets to keep the cycle going.
        // Jackpot random letters
        // Fill some bar/jar/whatever for mini game
        // Implement anonymous login with Playfab

        // Auto spin until possible win.
        // On Level 1 only level0 wins are possible. ETC.
        // Questionmark(?) help button. Flashes when 3 are possible. Flashes like crazy whe never used.
        // Flashes slightly less per level until level X. When you push it at level X you will be
        // notified that you can do it on your own yay! Also another discreet indicator when 3 are
        // possible.

        public void GetIconDisplayProbabilities(Credentials client, Action<GameServerResult, List<IconProbability>> callback)
        {
            callback(GameServerResult.Success, probabilitiesStatic_);
        }

        public void GetRollSequence(Credentials client, Action<GameServerResult, List<Roll>> callback)
        {
            List<Roll> result = new List<Roll>();
            for (int i = 0; i < 4; ++i)
            {
                List<int> final9 = new List<int>();
                PotentialWins.Build9(final9, probabilitiesStatic_);

                //PotentialWins.Build9WithGuarenteedMatch(
                //    PotentialWins.Difficulty.Level2_Diagonal_Nudge,
                //    final9,
                //    probabilitiesStatic_);

                Roll roll = new Roll()
                {
                    BatchId = rnd_.Next() << 32 + rnd_.Next(),
                    Icons = final9
                };
                result.Add(roll);
            }

            callback(GameServerResult.Success, result);
        }
    }
}
