# MapleSyrup
MapleSyrup, for the lack of a better name, is a C# MapleStory client emulator. 
This project was created for the love of the game, and interest in understanding its fundamentals. 
This project does not aim to be commercialized in any way, it's released to the community for those who also 
enjoy undertaking projects such as this.

### What's new? (02/05/2025)
- **Little-to-no-dependencies**:
  - The focus of the rework is to prevent what occurred with V1, extreme dependency. While there will still be some coupling, especially with the Singleton Pattern Factories, it won't be as bad as V1.
- Entity-Component-System:
  - Previously was an Actor-based system, which coupled the Data and Logic of the game object in one place, this caused a extreme about of issues with inheritance as well as with up keep. That no longer exists, a very basic ECS has been created, all creation logic is located in the EntityFactory, and no longer is the logic and data coupled together.
  While full established in the codebase, there are a few nuances with the current implementation, specially calling out the iteration and acquisition of the entities. Depending on the usage, it may consume a lot of memory in a short amount of time.
- **Scene-based control**:
  - Still in its infancy, the new scene-base controls will allow you to load maps with ease, no more calling various actors and manager to get things done. Adding and removing any objects from the scene is a lot more streamline as well.
- **Factory-based control**:
  - Before V1 HEAVILY relied on all the managers to the point things got placed where they shouldn't. Refactoring became a nightmare, and it was very difficult to determine who is responsible for what. With the new factories, they're focused on a "single purpose" model, as it should've been from the beginning.
- **Resource caching**:
  - Loading a lot of textures can be consuming for both the CPU and GPU, so the V2 client caches the textures that are in use and makes the reusable. So, specifically with tile nodes, you're no longer loading 100+ textures into memory, only the 10 or so variations each map uses.
  - Various Resource Types will be created as time goes on, these will be items such as the sprites, animation, and mapped items such as the equipment a player wears. This will allow a separation between what the ECS needs to know and not know.
  With the ResourceFactory comes the possibility of custom content, since the items are referenced by their specified path, not location in the NX, means you can store custom content in a separate directory and load it on client start. (NOT IMPLEMENTED).

## Libraries used
- Raylib-Charp-Vinculum (For gameplay visuals)
- K4os.Compression.LZ4 (For nx parsing)