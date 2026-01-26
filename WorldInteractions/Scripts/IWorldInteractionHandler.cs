namespace NightBlade
{
	/// <summary>
	/// Implemented by systems that handle a specific world interaction type.
	/// </summary>
	public interface IWorldInteractionHandler
	{
		WorldInteractType InteractType { get; }

		bool CanInteract(
			BasePlayerCharacterEntity player,
			int worldObjectId);

		void HandleInteract(
			BasePlayerCharacterEntity player,
			int worldObjectId);
	}
}
