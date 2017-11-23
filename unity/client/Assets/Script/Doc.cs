﻿// https://www.emojione.com/developers/download

// Random thought: Paint city. Choose a team color. Can only paint relatively small areas. One tile level
// might be enough. Copy them to own server and GTG. Works inside a certain radius. Can show on map.
// Can edit tiles directly in bitmap. Should be ok.

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
// - awesome idea: Show the donee. This increases donee exposure.
// NEAR NEAR MISS - roll one or two icons with no chance of win, but show donee.

// Gamesparks or Playfab?
// Spillet hvor alle vinder
// Vi er ikke som de andre - vi er noget for os selv - vi spiller spil på nettet, men vinder alligevel

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

// Let sponsor logos/donating player names move across the screen now and then, nonintrusive

// -- Summary --
// Credits
// Adjustable bet (maybe auto)
// Lottery ticket for offline spinning every x seconds. Can group wins when displaying.
//    Win: Credits, tickets, letters?, kroner? Always a few tickets to keep the cycle going.
//    Tickets may be the most important factor to make people come back! But it might conflict
//    with game itself.Remember: Don't make the game a barrier for the real fn!
// Jackpot random letters
// Fill some bar/jar/whatever for mini game
// Implement anonymous login with Playfab

// Auto spin until possible win.
// On Level 1 only level0 wins are possible. ETC.
// Questionmark(?) help button. Flashes when 3 are possible. Flashes like crazy whe never used.
// Flashes slightly less per level until level X. When you push it at level X you will be
// notified that you can do it on your own yay! Also another discreet indicator when 3 are
// possible.

// Add single/not single status? Really, really easy and people have pictures + Facebook already... Guys wants women who plays games.