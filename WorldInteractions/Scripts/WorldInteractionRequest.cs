namespace NightBlade
{
	/// <summary>
	/// Client → Server request for a world interaction.
	/// IDs only. No trust.
	/// </summary>
	public struct WorldInteractionRequest
	{
		public uint PlayerObjectId;
		public int WorldObjectId;
		public WorldInteractType InteractType;
	}
}
