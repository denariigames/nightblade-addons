using UnityEngine;
using UnityEngine.UI;

namespace NightBlade
{
	/// <summary>
	/// HUD-based interaction prompt for non-entity world interactables.
	/// </summary>
	public sealed class UITargetInteractable : UIBase
	{
		[Header("UI References")]
		[SerializeField] private Canvas canvas;
		[SerializeField] private Text promptText;

		private IInteractableTarget _current;
		private BasePlayerCharacterController _controller;

		protected override void Awake()
		{
			base.Awake();
			_controller = BasePlayerCharacterController.Singleton;
			Hide();
		}

		private void Update()
		{
			if (_current == null || _controller == null)
				return;

			//bool canInteract = _current.CanInteract(_controller);
			//if (promptText != null)
			//{
			//	promptText.text = canInteract  ? "Press F to Interact"  : "Too Far";
			//}
		}

		/// <summary>
		/// Called by UISceneGameplay when an interactable becomes the active target.
		/// </summary>
		public void SetTarget(IInteractableTarget interactable)
		{
			_current = interactable;

			if (_current == null)
			{
				Hide();
				return;
			}

			if (promptText != null)
			{
				promptText.text = _current.Title;
			}
			Show();
		}

		/// <summary>
		/// integrates with UIBase lifecycle.
		/// </summary>
		public override void Show()
		{
			if (canvas != null)
				canvas.enabled = true;

			base.Show();
		}

		/// <summary>
		/// integrates with UIBase lifecycle.
		/// </summary>
		public override void Hide()
		{
			if (canvas != null)
				canvas.enabled = false;

			base.Hide();
		}
	}
}
