# Freeplay Toolkit for VTOL:VR

Semi-configurable trainers for VTOL:VR for those who are like me and are bad at the game.

## Dependencies

This mod requires [VTOLAPI mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3265689427). Make sure to load that mod first before loading this mod.

## Configuration

The mode currently cannot be configured inside the game. Instead you need to go to the modloader settings directory at wherever the game and modloader is installed (should look something like `steamapps\common\VTOL VR\@Mod Loader\Settings\`), and under the `ccw.FreeplayToolkitV2` directory. After loading the mod for the first time, there will be 3 files that are settings files which can be modified to configure the settings for the mod.

Once the mod loader's in game settings menu returns I will add configurability in game for this mod.


## Magic Pockets (In Flight Reloads)

Ever play games like Ace Combat, Project Wingman, and even as Tom Clancy H.A.W.X 2, only to be dismayed by the fact that fighters do not in fact, have the ability to magically reload their missiles mid-flight?

Well the Magic Pockets is for you. The HUD also displays how many spare munitions are in your magic pockets.

Countermeasures (Chaff and Flares) are also automatically reloaded.

You can specify how many reloads per hardpoint you want in the `MunitionsModifier.toml` file in the settings directory. Note, there are other fields in the setting that has no effect. They will not be listed below.
* `ReloadTime` - `float` (default: 15.0): Sets the reload delay in seconds after firing munitions.
* `ReloadCount` - `int` (default: 0): The number of reloads in hardpoint worth. Ie; if a hardpoint mounts 2 missiles, and this number is set to 5, you will have spare 10 missiles for that hardpoint.
* `InfiniteAmmo` - `bool` (default: false): Whether infinite ammo is enabled or not. If this is set to true, `ReloadCount` is ignored.

## Fuel Modifier

Want to keep afterburners on but don't want the baggage of external fuel tanks? A simple fuel consumption modifier can be set in `FuelModifier.toml`.
* `FuelDrainModifier` - `float` (default, 1.0): Fuel consumption modifier, 1 is default/vanilla, 0 means no fuel consumption.

## Damage Modifiers

Suck at dodging missiles? Annoyed allies can't defend from missiles if their lives depend on it? Or just want to make the enemies harder to kill because you now have 500 missiles in your magic pockets? These configurations allow you to modify how much damage you, your allies, or your enemies take from different damage sources. 

Configurations can be set in `DamageModifier.toml`
* `AllyDamageMultiplier` - `float` (default, 1.0): Multiplier on the incoming damage for allies
* `SelfDamageMultiplier` - `float` (default, 1.0): Multiplier on the incoming damage for you the player
* `EnemyDamageMultiplier` - `float` (default, 1.0): Multiplier on the incoming damage for enemies.
* For the settings above, 1.0 means 1x incoming damage, 0.0 means no damage will be taken, etc.


## Future planned features

Will get around to these when I get around to them.


## Quirks and Known Bugs

### Magic Pockets

* Bug where sometimes munitions cannot be used after reload until all hardpoints have been reloaded. Ie, if you have 2 hardpoints, fired off 2 missiles, you won't be able to fire that munition until both hardpoints have been reloaded.
* When using guns, neither HUD nor HMD shows amount of bullets left in the Magic Pocket.

### Fuel Modifiers

* Currently only works on the player's fuel consumption

### Damage Modifiers

* Although the plane itself will suffer now damage, the player can still die (ie due to G force from a CFIT).