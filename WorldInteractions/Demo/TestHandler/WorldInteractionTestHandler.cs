using UnityEngine;

namespace NightBlade
{
	/// <summary>
	/// Temporary test handler to validate world interaction pipeline.
	/// </summary>
	public sealed class WorldInteractionTestHandler : MonoBehaviour, IWorldInteractionHandler
	{
		public WorldInteractType InteractType => WorldInteractType.Test;

		public bool CanInteract(BasePlayerCharacterEntity player, int worldObjectId)
		{
			Debug.Log($"[TestHandler] CanInteract id={worldObjectId}");
			return true;
		}

		public void HandleInteract(BasePlayerCharacterEntity player, int worldObjectId)
		{
			Debug.Log($"[TestHandler] HandleInteract id={worldObjectId}");
		}
	}
}
