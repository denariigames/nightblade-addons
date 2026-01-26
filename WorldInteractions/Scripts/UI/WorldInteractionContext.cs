using System;
using UnityEngine;

namespace NightBlade
{
	/// <summary>
	/// Provides transient, hit-based context for non-networked world interactions.
	/// Tracks what IInteractableTarget the player is currently hovering.
	/// </summary>
	public sealed class WorldInteractionContext
	{
		/// <summary>
		/// Interactable currently under the cursor (this frame).
		/// Null if none.
		/// </summary>
		public IInteractableTarget Current { get; private set; }

		/// <summary>
		/// Interactable hovered in the previous frame.
		/// Null if none.
		/// </summary>
		public IInteractableTarget Previous { get; private set; }

		/// <summary>
		/// True if an interactable is currently hovered.
		/// </summary>
		public bool HasInteractable => Current != null;

		/// <summary>
		/// True if the currently hovered interactable can be interacted with.
		/// </summary>
		public bool CanInteract { get; private set; }

		/// <summary>
		/// Fired when hover enters a new interactable.
		/// </summary>
		public event Action<IInteractableTarget> OnHoverEnter;

		/// <summary>
		/// Fired when hover exits an interactable.
		/// </summary>
		public event Action<IInteractableTarget> OnHoverExit;

		/// <summary>
		/// Fired whenever hover target changes (enter or exit).
		/// </summary>
		public event Action<IInteractableTarget> OnHoverChanged;

		/// <summary>
		/// Update the hover context from a raycast hit.
		/// Call once per frame.
		/// </summary>
		public void UpdateFromHit(
			Transform hitTransform,
			BasePlayerCharacterController controller)
		{
			IInteractableTarget next = null;

			if (hitTransform != null)
			{
				next = hitTransform.GetComponentInParent<IInteractableTarget>();
			}

			if (ReferenceEquals(Current, next))
			{
				// Still hovering the same interactable
				UpdateCanInteract(controller);
				return;
			}

			// Hover exit
			if (Current != null)
			{
				OnHoverExit?.Invoke(Current);
			}

			Previous = Current;
			Current = next;

			UpdateCanInteract(controller);

			// Hover enter
			if (Current != null)
			{
				OnHoverEnter?.Invoke(Current);
			}

			OnHoverChanged?.Invoke(Current);
		}

		/// <summary>
		/// Returns true if the given interactable is currently hovered.
		/// </summary>
		public bool IsHovering(IInteractableTarget interactable)
		{
			return ReferenceEquals(Current, interactable);
		}

		/// <summary>
		/// Clears the current hover state and fires exit events if needed.
		/// </summary>
		public void Clear()
		{
			if (Current != null)
			{
				OnHoverExit?.Invoke(Current);
				OnHoverChanged?.Invoke(null);
			}

			Previous = Current;
			Current = null;
			CanInteract = false;
		}

		private void UpdateCanInteract(BasePlayerCharacterController controller)
		{
			if (Current == null || controller == null)
			{
				CanInteract = false;
				return;
			}

			CanInteract = Current.CanInteract(controller);
		}
	}
}
