# MyCharacter
A bunch of systems I am currently working on
 
## Character Controller using a HFSM

## Questing System
- Data Driven Design (using Scriptable Objects)
    - Easy to create quests, edit requirements, steps, rewards all from Unity inspector
- Flexible Events Systems that is easily pluggable
    - As an example, the Quest system is configured to work with the Inventory System using a simple script

## Save System
- Save & load game data using JSON files
- Easily configure data to be saved 
    - As an example, the Save system is configured to save Inventory data & some Player Data
- Multiple Saves supported
- Separation of concerns so that serializing method, data persistance method can be easily swapped
- (TODO) Encryption
- (TODO) Cloud Save Option

## Inventory System 
- Separation of Concerns between Inventory data and UI
    - Able to have multiple inventory views. Sample has one view for hotbar and one view for a full inventory which can be toggled.
    - Sample hotbar with hotbar selector, allows using selected item (if custom item behaviour is created for it)
- Easily load inventory from static data
- Drag and drop to move items around 
- Add/remove multiple items at once
- Adhere to stack limit for items

### Item System
- As for now, part of the Inventory System
- Item Database Scriptable Object holds a list of all item data (scriptable objects) in the game
- ItemUtils allows retrieval of item data (SO) or item behaviour scripts
#### Item
- An item can have 2 parts: The data (Scriptable Object) and the behaviour (optional C# script)
    - Creating an item is as simple as creating a ScriptableObject for it and adding it to the Item Database
    - Able to create custom item behaviour script for it if needed (e.g. to implement functionality when item is used)


## Custom Notification Pop up system
- 2 second timer
- Allows multiple pop ups
