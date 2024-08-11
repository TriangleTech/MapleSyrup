# MapleSyrup
MapleSyrup, for the lack of a better name, is a C# MapleStory client emulator. 
This project was created for the love of the game, and interest in understanding its fundamentals. 
This project does not aim to be commercialized in any way, it's released to the community for those who also 
enjoy undertaking projects such as this. This client has been re-written from the ground up, so it's back to 
square one. Is it discouraging to start over? No, it's actually pretty fun. 
 
Legality: I'm not claiming rights or ownerships over any images used or the name "MapleStory".
All rights and trademarks of MapleStory go to their respective owners. 

## What's being worked on? (For details check Changelog)
- Login Screen (No transition to world, yet)
- Character Rendering using Server.
- Character Creation (Next in list)

## Compactibility 
- Maplestory Beta (v40b): The client now support Data.nx files, I did test it out a bit and works just fine.
- Version XX thru v75??: What I call the *Legacy Layout* for the login screen is nearly complete. This is basically the old
school version that 55, 62, etc. had. I believe it had a slight change when Cygnus was introduced.
- Version 83 - 92??: This layout has not been tested at all, so use at your own risk.
- Version 95 thru Zero??: The *post-bb* layout, before they added more characters is partially supported. It only works until the Character
select screen, but nothing is rendered there.

Eventually I do want to add an internal editor, this will allow you to add whatever you want into the NX files without having to swap them in and out
and worrying if it's going to be broken in game.

## Building
### Building
>Clone repo.
>Go to AppConfig and change any server related info, do not touch the ratio calcuations, you'll break everything. 
>Build.

### Using Networking (Updated 8.9.24)
I reverted the packet header and opcode reading process. It's back to the original format of reading the packet length and opcode.
After it looks for the remaining data. Nothing is perfect right now, it doesn't even check if that data is valid.

Afterwards, the client will connect to the server and begin sending data back and forth.

Please note, that I stripped a lot of the excess BS that the server have, either add it into the packets, or match what I have.

## Libraries used
I develop primary on Linux, so this should work with no issues on windows. I do test it from time to time on windows.
- Raylib-csLo (The other one works too but lacks a lot of the other libraries.)

## What happened before?
Originally in June, accidentally put August, I honestly have no excuse as to why...I stated I would be moving 
forward with a C++ client, I decided to keep that under wraps and transfer what I learned in that client to 
this one. My schedule has also become somewhat stable now, so there won't be months in-between updates anymore.

Loading times have been decreased, larger maps still take about 3 to 7 seconds to load, but that's better than 
the previous 25 seconds that it took. It could be better, but I haven't bothered to optimize anything yet.
