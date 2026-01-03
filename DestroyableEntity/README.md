# Destroyable Entity

<img src="https://github.com/denariigames/nightblade-addons/blob/master/DestroyableEntity/screenshot.png" alt="DestroyableEntity" height="350">

An addon for NightBlade MMO that adds new HarvestableEntity type that awards only on death (as opposed to per hit). Useful for things like smashing barrels.

- adds optional configuration to always use items in the first weapon. No need to duplicate across weapons or skills.

## Usage

1. edit BarrelHarvestable for drops inside barrel when destroyed
2. assign BarrelHarvestable to BarrelHarvestableEntity
3. (optional) check *Use Items From First Weapon* if you want all weapon types to have equal damage effectiveness and drops. Note that the weapon type will need a Harvest Damage amount. The only weapon types in the default demo with Harvest Damage amount assigned are the Axe and StonePick.
4. assign BarrelDestroyableEntity to a HarvestableSpawnArea
5. add BarrelHarvestable to GameDatabase

## Credits

- Barrel model in demo from [Quaternius RPG Essentials Pack](https://quaternius.com/packs/rpg.html) (License CC0)
