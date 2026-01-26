using System.Collections.Generic;
using UnityEngine;

namespace NightBlade
{
	[System.Serializable]
	public struct WorldInteractableUseItem
	{
		public int worldId;
		public BaseItem item;
		public int amount;
	}

	/// <summary>
	/// Sample Use handler.
	/// </summary>
	public sealed class WorldInteractionUseHandler : MonoBehaviour, IWorldInteractionHandler
	{
		public WorldInteractType InteractType => WorldInteractType.Use;
		public WorldInteractableUseItem[] useItems;
		private Dictionary<int, WorldInteractableUseItem> _cacheUseItems = new Dictionary<int, WorldInteractableUseItem>();

		/// <summary>
		/// Build cache
		/// </summary>
		void Awake()
		{
			foreach (WorldInteractableUseItem WorldInteractableUseItem in useItems)
			{
				_cacheUseItems.Add(WorldInteractableUseItem.worldId, WorldInteractableUseItem);
			}
		}

		/// <summary>
		/// IWorldInteractionHandler handler
		/// </summary>
		/// <param name="player"></param>
		/// <param name="worldObjectId"></param>
		public bool CanInteract(BasePlayerCharacterEntity player, int worldObjectId)
		{
			if (!player.CanPickup())
				return false;
			return true;
		}

		/// <summary>
		/// IWorldInteractionHandler handler
		/// </summary>
		/// <param name="player"></param>
		/// <param name="worldObjectId"></param>
		public void HandleInteract(BasePlayerCharacterEntity player, int worldObjectId)
		{
			WorldInteractableUseItem useItem;
			if (TryGetUseItem(worldObjectId, out useItem))
				player.CallCmdUseWorldItem(worldObjectId);
		}

		/// <summary>
		/// Checks cache for valid WorldInteractableUseItem.
		/// </summary>
		/// <param name="worldObjectId"></param>
		public bool TryGetUseItem(int worldObjectId, out WorldInteractableUseItem useItem)
		{
			if (_cacheUseItems.TryGetValue(worldObjectId, out useItem))
			{
				if (useItem.item != null && useItem.amount > 0)
				{
					return true;
				}
			}

			useItem = new WorldInteractableUseItem();
			return false;
		}
	}
}
