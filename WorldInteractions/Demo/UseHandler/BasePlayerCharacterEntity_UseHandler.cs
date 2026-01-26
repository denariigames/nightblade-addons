using LiteNetLibManager;
using UnityEngine;

namespace NightBlade
{
	public abstract partial class BasePlayerCharacterEntity
	{
		/// <summary>
		/// Callback from WorldInteractionUseHandler
		/// </summary>
		/// <param name="targetItemId"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public bool CallCmdUseWorldItem(int worldObjectId)
		{
			//client side checks before issuing RPC
			if (!CanPickup())
				return false;

			IWorldInteractionHandler handler = InteractionManager.Instance.GetHandler(WorldInteractType.Use);
			if (handler == null)
				return false;

			WorldInteractableUseItem useItem;
			if ((handler as WorldInteractionUseHandler).TryGetUseItem(worldObjectId, out useItem))
			{
				RPC(CmdWorldInteractionUseItem, worldObjectId);
				CallRpcPlayPickupAnimation();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Called at server for character to receive item
		/// </summary>
		[ServerRpc]
		protected virtual void CmdWorldInteractionUseItem(int worldObjectId)
		{
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
			//same checks on server
			if (!CanPickup())
				return;

			IWorldInteractionHandler handler = InteractionManager.Instance.GetHandler(WorldInteractType.Use);
			if (handler == null)
				return;

			WorldInteractableUseItem useItem;
			if ((handler as WorldInteractionUseHandler).TryGetUseItem(worldObjectId, out useItem))
			{
				if (GameInstance.Items.TryGetValue(useItem.item.DataId, out BaseItem targetItem))
				{
					if (this.IncreasingItemsWillOverwhelming(targetItem.DataId, useItem.amount))
					{
						GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
						return;
					}

					GameInstance.ServerGameMessageHandlers.NotifyRewardItem(ConnectionId, RewardGivenType.Harvestable, targetItem.DataId, useItem.amount);
					this.IncreaseItems(CharacterItem.Create(targetItem.DataId, 1, useItem.amount));
				}
			}
#endif
		}
	}
}