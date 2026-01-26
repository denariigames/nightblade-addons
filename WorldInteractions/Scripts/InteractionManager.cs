using System.Collections.Generic;
using UnityEngine;

namespace NightBlade
{
	/// <summary>
	/// Server-authoritative router for all non-entity world interactions.
	/// Stateless by design. 
	/// 
	/// Usage: add one more more IWorldInteractionHandler scripts as children.
	/// </summary>
	public sealed class InteractionManager : MonoBehaviour
	{
		public static InteractionManager Instance { get; private set; }
		private readonly Dictionary<WorldInteractType, IWorldInteractionHandler> handlers = new Dictionary<WorldInteractType, IWorldInteractionHandler>();

		/// <summary>
		/// Singleton pattern
		/// </summary>
		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);
			RegisterHandlers();
		}

		/// <summary>
		/// Register all handlers that are children of the InteractionManager
		/// </summary>
		private void RegisterHandlers()
		{
			handlers.Clear();

			IWorldInteractionHandler[] found = GetComponentsInChildren<IWorldInteractionHandler>(true);
			foreach (var handler in found)
			{
				handlers[handler.InteractType] = handler;
			}
		}

		/// <summary>
		/// Return a handler by WorldInteractType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IWorldInteractionHandler GetHandler(WorldInteractType type)
		{
			if (handlers.TryGetValue(type, out var handler))
				return handler;

			return null;
		}

		/// <summary>
		/// Entry point called by network message / command.
		/// Player resolution is intentionally external.
		/// </summary>
		public void HandleInteractionRequest(BasePlayerCharacterEntity player, WorldInteractionRequest request)
		{
			if (player == null || player.IsDead())
				return;

			if (!handlers.TryGetValue(request.InteractType, out var handler))
				return;

			if (!handler.CanInteract(player, request.WorldObjectId))
				return;

			handler.HandleInteract(player, request.WorldObjectId);
		}
	}
}
