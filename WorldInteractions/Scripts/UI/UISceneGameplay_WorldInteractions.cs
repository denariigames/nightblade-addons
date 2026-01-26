using UnityEngine;
using UnityEngine.Serialization;

namespace NightBlade
{
	public partial class UISceneGameplay
	{
		[SerializeField] private UITargetInteractable uiTargetInteractable;

		public override void SetTargetInteractable(IInteractableTarget interactable)
		{
			if (uiTargetInteractable == null)
				return;

			uiTargetInteractable.SetTarget(interactable);
		}
	}
}