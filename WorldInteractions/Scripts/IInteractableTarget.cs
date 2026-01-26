using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightBlade
{
	/// <summary>
	/// Indicates a non-entity world object that supports
	/// movement-based interaction (walk to, stop, interact).
	///
	/// This does NOT imply:
	/// - networking
	/// - combat
	/// - damage
	/// - persistence
	///
	/// It is purely a gameplay interaction contract.
	/// </summary>
	public interface IInteractableTarget : ITargetableEntity
	{
		string DefaultTitle { get; set; }
		LanguageData[] LanguageSpecificTitles { get; set; }
		string Title { get; }

		/// <summary>
		/// Returns whether the player is currently allowed
		/// to interact with this target.
		/// Used by hover, UI feedback, and validation.
		/// </summary>
		bool CanInteract(BasePlayerCharacterController controller);

		/// <summary>
		/// Distance at which the player should stop
		/// and trigger the interaction.
		/// </summary>
		float GetInteractDistance();

		/// <summary>
		/// Called once when the player reaches the interaction range.
		/// This should only dispatch intent (e.g., send a request),
		/// not apply authoritative state.
		/// </summary>
		void Interact(BasePlayerCharacterController controller);
	}
}
