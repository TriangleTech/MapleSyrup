# MapleSyrup
MapleSyrup, for the lack of a better name, is a C# MapleStory client emulator. 
This project was created for the love of the game, and interest in understanding its fundamentals. 
This project does not aim to be commercialized in any way, it's released to the community for those who also 
enjoy undertaking projects such as this.

## Changes
The previous version of the client, currently in the master branch at this time, had many issues. Some were easy 
to spot, others were made apparent by the community. After careful consideration I wiped the project clean and 
started over.

### What's new? (02/05/2025)
- Entity-Component-System. This uses an ID-based ECS that is effective for the needs of the project.
- Singleton-Pattern Factories. No longer do you have to deal with a service locator, simply call the class and its instance.
- Clean, easy to follow, codebase. Before I didn't take any of that to account, I've been more careful this time.
- Revamped NX Parser. This parser is written similar to my parser written in 'C99', you can tell, but it does made it easy to use and understand.
- Faster load times. Before loading took a bit of time, but it's fair quick this time around.
- No networking. Yes, I completely got rid of the network-side, if you want it back you'll have to add it.
- Emulation focused client. This client no longer aims to be interactable with a server, sorry.

## Libraries used
I develop primary on Linux, so this should work with no issues on windows. I do test it from time to time on windows.
- Raylib-csLo (The other one works too but lacks a lot of the other libraries.)