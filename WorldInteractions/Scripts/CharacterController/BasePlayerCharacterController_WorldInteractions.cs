using UnityEngine;

namespace NightBlade
{
	public partial class BasePlayerCharacterController
	{
		/// <summary>
		/// World interaction hover context.
		/// Passive: fed by existing raycast results.
		/// </summary>
		private readonly WorldInteractionContext _worldInteractionContext = new WorldInteractionContext();

		/// <summary>
		/// MUST be called whenever a world raycast result is available,
		/// regardless of whether a click occurred.
		/// This wires hover detection without a new Update loop.
		/// </summary>
		private void UpdateWorldInteractionContext(Transform hitTransform)
		{
			_worldInteractionContext.UpdateFromHit(hitTransform, this);
		}

		/// <summary>
		/// One-time initialization for World Interaction feedback hooks.
		/// Call during controller setup.
		/// </summary>
		private void InitializeWorldInteractionFeedback()
		{
			// -------------------------
			// Debug / validation
			// -------------------------
			_worldInteractionContext.OnHoverEnter += target => Debug.Log($"[WorldInteraction] Hover ENTER: {target.GetType().Name}");

			_worldInteractionContext.OnHoverExit += target => Debug.Log($"[WorldInteraction] Hover EXIT: {target.GetType().Name}");

			// -------------------------
			// Temporary hover color
			// -------------------------
			_worldInteractionContext.OnHoverEnter += target =>
			{
				var hoverColor = (target as MonoBehaviour)?.GetComponentInParent<WorldInteractionHoverColor>();
				hoverColor?.OnHoverEnter();
			};

			_worldInteractionContext.OnHoverExit += target =>
			{
				var hoverColor = (target as MonoBehaviour)?.GetComponentInParent<WorldInteractionHoverColor>();
				hoverColor?.OnHoverExit();
			};

			// -------------------------
			// Cursor feedback (STUB)
			// -------------------------
			_worldInteractionContext.OnHoverChanged += target =>
			{
				// TODO:
				// if (_worldInteractionContext.CanInteract)
				//     SetInteractCursor();
				// else
				//     SetDefaultCursor();
			};

			// -------------------------
			// Outline / highlight (STUB)
			// -------------------------
			_worldInteractionContext.OnHoverEnter += target =>
			{
				// TODO: enable outline
			};

			_worldInteractionContext.OnHoverExit += target =>
			{
				// TODO: disable outline
			};

			// -------------------------
			// Tooltip / UI prompt (STUB)
			// -------------------------
			_worldInteractionContext.OnHoverChanged += target =>
			{
				// TODO:
				// Show / hide UI prompt
				// bool show = _worldInteractionContext.CanInteract;
			};
		}
	}
}
