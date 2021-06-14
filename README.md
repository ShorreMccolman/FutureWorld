
## **Welcome to Future World!**
This is a "remake" of one of my favorite games - Might & Magic VI: The Mandate of Heaven. This started as a for-fun exercise in remaking the Inventory system of the game but as I expanded it outwards I decided to challenge myself to recreate the starting area of the game in most of its entirety. Along the way I decided to make a number of changes to the controls and UI of the game but for the most part I have tried to keep the recreation faithful when possible. My plan is to recreate the town of New Sorpigal and its surrounding area along with most of the gameplay features of M&M VI. My hope is that this might generate some interest with others who might wish to collaborate on an original game in the same style.

The vast majority of assets in the game were created by me using various tools with a few exceptions. The buildings and terrain were created by my brother. The 3D characters were either made by me with MakeHuman or downloaded from Mixamo.com. The animations in game are currently all taken from that same site. The sounds are from a library I purchased a liscense for. The music is taken directly from the original game and is being used as a placeholder for now. There are some more 3rd party models and textures but for the most part the artwork was created by me based on designs from the original game.

Please keep in mind I do not advertise myself as an artist. I am doing it myself because there is nobody else to do it. A lot of is very bad, I know... try to use your imagination lol.

All of the code with very minor exceptions was written by me. I did not copy any of the original games scripts, everything has been recreated from scratch as my own interpretation. However I did study the mechanics of the game very closely and had discussions with community members about the details of a few of the hidden systems. The project is certainly a work in progess, but many systems are already in place.


### **Create your Party**
![Screenshot (1)](https://user-images.githubusercontent.com/29645590/121886095-3c316280-ccd2-11eb-993d-d0f0bbf9b497.png)

Customize your party members. Portrait, name, class, stats and skills. Each character starts with 4 skills, 2 are determined by class and 2 are of your choosing. Become a member of one of the guilds in town and you can train new skills for a price. You also get 50 points to spend on stats - you dont have to spend them evenly so you can stack a single character if you really want to. Your skills will determine the items each character starts the game with. Choose quick start to start with a premade fully equipped party.

### **Equip your party and collect loot**
![Screenshot (9)](https://user-images.githubusercontent.com/29645590/121888406-312c0180-ccd5-11eb-8bfe-7b6fe72e800d.png)

Each character has a seperate inventory that stores items of various sizes on a grid. Items can be equipped to your player just by dragging them and dropping them over your portrait. Most weapons and armor are only equippable if the character knows the relevant skill but things like cloaks and helmets are available for all characters. Right click any item to read about it. There are nearly 200 items of various types available in game - find them in chests, shops or out in the world. Manage your inventory by trading items between characters. 

![Screenshot (4)](https://user-images.githubusercontent.com/29645590/121888526-59b3fb80-ccd5-11eb-999c-ee788b67caea.png)

Chests will intelligently spawn random loot and gold. Items are weighted at different tiers so that low quality chests have higher chances of spawning lower quality loot. The same items can appear at different tier levels but with different frequencies for each tier they can be spawned in at. Weapons and Armor come in a variety of subtypes and have different enchantments that can appear on them depending on type and tier level.

![Screenshot (10)](https://user-images.githubusercontent.com/29645590/121889270-46556000-ccd6-11eb-8d0b-e7988d0dbb03.png)

Merchants will stock items depending on the type of shop and will restock their shelves with random items at least once per week depending on the type of shop. Blacksmiths, Armorsmiths and Alchemists can repair or identify the kind of items that they sell. Most shops offer a selection of lower quality items as well as a seperate selection of higher quality items, usually of a specific type.

### **Interact with the townsfolk**
![Screenshot (5)](https://user-images.githubusercontent.com/29645590/121889832-ec08cf00-ccd6-11eb-8bd3-be58e19e6f91.png)

The town is populated with random NPCs who walk around town offering lore and services. Speak with them to find out what they can offer and hire up to 2 to travel with your party. They will take a small percentage of the gold you find as well as an up front payment. NPCs can also be found inside houses where they offer a variety of services such as the bank or inn. These NPCs can also sometimes offer quests for the party to go on.

### **Fight in real time or turn based mode**
![Screenshot (8)](https://user-images.githubusercontent.com/29645590/121890495-a8fb2b80-ccd7-11eb-8959-35f2ab165a78.png)

Fight with monsters using your equipment and skills. Killing them earns experience which you can use to level up your characters and improve their skills. You will soon also be able to cast spells for more interesting combat options. Learn new spells by finding or purchasing spell books or cast them one time with a scroll.

### **In game clock and calender**
![Screenshot (12)](https://user-images.githubusercontent.com/29645590/121890849-08593b80-ccd8-11eb-90f0-a1714d7ebb15.png)

The game runs on an in game clock and calendar that controls everything from the day night cycle to status effects and cooldowns. Open the rest menu and consume food to give your party a proper rest or just to wait time out quickly. If your characters go too long without being rested however they will become weak. 

Stop the clock at any time by pressing enter. This will change the combat to a turn based style where you and the enemies take turns attacking and moving.

### **Save at any time**

You can open the menu and save at any time. Everything is serialized into a human readable file for easy debugging. Reload your file to find the scene recreated how you left it. Keep multiple save files for a single playthrough or have several playthroughs going at once.
