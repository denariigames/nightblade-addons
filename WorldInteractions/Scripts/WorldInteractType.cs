namespace NightBlade
{
	/// <summary>
	/// Identifies the type of world interaction being requested.
	/// Used only for routing.
	/// </summary>
	public enum WorldInteractType
	{
		Test,

		// Resource Gathering
		Harvest,    // Crops, plants, berries
		Mine,       // Ore veins, crystals
		Chop,       // Trees, logs
		Fish,       // Water sources
		Skin,       // Animal corpses

		// Social & NPC
		Talk,       // Dialogue, quests

		// Item/Object Usage
		Open,       // Doors, chests, gates
		Use,        // Consume/apply item on object
		Activate,   // Levers, buttons, runes
		Craft,      // Workbench, forge, alchemy table

		// Utility & Exploration
		Pickup,     // Ground items
		Examine,    // Inspect for lore/info
		Sit,        // Benches, chairs (RP/rest)
		Climb,      // Ladders, ropes
		Custom,
	}
}
