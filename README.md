# MapleSyrup
MapleSyrup, for the lack of a better name, is a C# MapleStory client emulator. 
This project was created for the love of the game, and interest in understanding its fundamentals. 
This project does not aim to be commercialized in any way, it's released to the community for those who also 
enjoy undertaking projects such as this. This client has been re-written from the ground up, so it's back to 
square one. Is it discouraging to start over? No, it's actually pretty fun. 
 
Legality: I'm not claiming rights or ownerships over any images used or the name "MapleStory".
All rights and trademarks of MapleStory go to their respective owners. 

## What happened before?
Originally in June, accidentally put August, I honestly have no excuse as to why...I stated I would be moving 
forward with a C++ client, I decided to keep that under wraps and transfer what I learned in that client to 
this one. My schedule has also become somewhat stable now, so there won't be months in-between updates anymore.

Loading times have been decreased, larger maps still take about 3 to 7 seconds to load, but that's better than 
the previous 25 seconds that it took. It could be better, but I haven't bothered to optimize anything yet.

## What's being worked on? (For details check Changelog)
- Networking (partially done).
- Map loading (via networking).
- Basic Avatar (Can change states, animate, blink aggressively)

## Building & Using Network-side.
### Building
- Go into the NXManager class and change the location of the NX files. I will make this more straightforward,
later.
- Restore Nuget Packages
- Build

### Using Networking
While I won't provide the server source I am using, can't give everything out. I will say that
it fairly simple and straightforward to grab a server source, of any kind, and modify it like I state
below. This client is create for fun and entertainment purposes, the server side is a plus, not a
requirement. 

SO, with that rant out of the way. There are two ways to use the networking in this client:
- Either implement the MS cryptography system, I refuse.
- Remove all AES & MapleCustomEncryption out of a server and have it read the header and data.

It's pretty obvious what I did, anyone with a junior-level of programming knowledge can get this done in about 5 minutes, so it's nothing difficult.

## Libraries used
I develop primary on Linux, so this should work with no issues on windows. I do test it from time to time on windows.
- Raylib-csLo (The other one works too but lacks a lot of the other libraries.)
