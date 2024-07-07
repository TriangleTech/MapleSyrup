# MapleSyrup
>MapleSyrup, for the lack of a better name, is a C# MapleStory client emulator. 
This project was created for the love of the game, and interest in understanding its fundamentals. 
This project does not aim to be commercialized in any way, it's released to the community for those who also 
enjoy undertaking projects such as this. This client has been re-written from the ground up, so it's back to 
square one. Is it discouraging to start over? No, it's actually pretty fun. 
 
> Legality: I'm not claiming rights or ownerships over any images used or the name "MapleStory".
> All rights and trademarks of MapleStory go to their respective owners. 

## What happened before?
>Originally in June, accidentally put August, I honestly have no excuse as to why...I stated I would be moving 
> forward with a C++ client, I decided to keep that under wraps and transfer what I learned in that client to 
> this one. My schedule has also become somewhat stable now, so there won't be months in-between updates anymore.

>Loading times have been decreased, larger maps still take about 3 to 7 seconds to load, but that's better than 
> the previous 25 seconds that it took. It could be better, but I haven't bothered to optimize anything yet.

## What's being worked on? (For details check Changelog)
>- Networking (partially done).
>- Map loading (via networking).

## Building & Using Network-side.
### Building
>I primarily use JetBrains' Rider IDE, so I'm familiar with how to compile on that. Simply, grab the project, restore
>the nuget packages, and build.
### Using Networking
>So, I'm more than happy to open source something like this, what I'm not going to do is completely hand over all the information. 
This will cause not only problems for me, but for those who are inexperienced and decide to do something stupid like releasing a 
PS without knowing how to modify information properly.

>SO, with that rant out of the way. There are two ways to use the networking in this client:
>- Either implement the MS cryptography system, I refuse.
>- Remove all AES & MapleCustomEncryption out of a server and have it read the header and data.

>It's pretty obvious what I did, anyone with a junior-level of programming knowledge can get this done in about 5 minutes, so it's nothing difficult.

## Libraries used
I develop primary on Linux, so this should work with no issues on windows. I do test it from time to time on windows.
- Raylib-csLo (The other one works too but lacks a lot of the other libraries.)