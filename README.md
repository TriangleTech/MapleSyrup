# Update (8/17/24)
I know, been a while since the last update. I changed the codebase and will be posting a link here soon, I have to fix some stuff first. The new codebase is in C++, but if you're familiar with how this one looks, you'll see they quite identical.

I didn't plan to change it, but C#'s processing is horrible. I made it a pure pointer base library at one point and still couldn't generate the biggest map faster than 25 seconds. 

It's written in clean C++ (C#-like C++), it still has the same features as the prior one, plus some. I still plan on continuing the C# client, but it'll most likely become a pure offline client. The C++ is also further ahead than this one.

Stay toned!

# MapleSyrup
MapleSyrup is a emulation client for MapleStory written in C#. This project is primarily for
educational purposes and is highly recommended not to be used for commercial purposes. Use
at your own risk.

**Note**: Take a look at the branches, sometimes I will be working on a feature or change that may
break the whole client, so I send it to an experimental branch. All branch names will be
*experimental_* + the features I am doing.

As of **01/10/2024** I am working on experimental_overhaul, it's much cleaner, in my opinion.

I will remove this note once work on the branch is complete.

## Features
Currently these are the features of the client with more to be added:

- Hybrid Entity-Component-System (Experimental).
- Various subsystems to control the engine.
- Game Context, containing the systems within the engine, avoiding the singleton pattern.
- Event-base communication system (Experimental).
- Resource System with zero abstraction to WZ or NX format, regardless of which
is used, it doesn't change the code base.

## TODO
- Map Rendering (60%) missing background scrolling, NPCs, and benches.
- Character Rendering (0%) *IN-WORK*
- Mob Rendering (0%)
- Networking (0%) NEXT IN LIST

## Notes
- This project to created to not copy the original game, or to be "MS-Like", but more to
create a flexible, extensible, and easy-to-use "engine" that features can be added as it 
gets complete.
- The primarily aim for this reason is to find alternative to the original methods MS used
to render or move players and objects. This is where the learning purposes comes in.
- While I will provide support with enhancements and bug fixes, support for any additional 
content after this project is considered "complete" will not be provided.
- This project works on priorities, so there'll be instances where things are **purposely** 
left incomplete for the sake of completion. This items will be minor and not affect gameplay.
 An example of this, background scrolling (i.e clouds, lights, etc.) are ignored for the sake
of progression. **It will be added at a later time** but it doesn't affect gameplay at all.
