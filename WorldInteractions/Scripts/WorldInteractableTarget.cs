using UnityEngine;

namespace NightBlade
{
	/// <summary>
	/// Non-entity world object that supports targeting, movement, stopping, and interaction.
	/// </summary>
	public sealed class WorldInteractableTarget : MonoBehaviour, IInteractableTarget
	{
		[Header("Interaction")]
		[SerializeField] private string defaultTitle = string.Empty;
		[SerializeField] private LanguageData[] languageSpecificTitles;
        public string DefaultTitle
        {
            get { return defaultTitle; }
            set { defaultTitle = value; }
        }
        public LanguageData[] LanguageSpecificTitles
        {
            get { return languageSpecificTitles; }
            set { languageSpecificTitles = value; }
        }
        public string Title
        {
            get { return Language.GetText(languageSpecificTitles, defaultTitle); }
        }

		[SerializeField] private float interactDistance = 2f;
		[SerializeField] private WorldInteractType interactType;

		[Header("Identity")]
		[SerializeField] private int worldObjectId;

		#region ITargetableEntity

		public Transform EntityTransform => transform;
		public GameObject EntityGameObject => gameObject;

		public bool SetAsTargetInOneClick() => true;
		public bool NotBeingSelectedOnClick() => false;

		#endregion

		#region IInteractableTarget

		InteractionManager manager;


		public bool CanInteract(BasePlayerCharacterController controller)
		{
			if (controller == null)
				return false;

			float sqrDist = (controller.transform.position - transform.position).sqrMagnitude;

			return sqrDist <= interactDistance * interactDistance;
		}

		public float GetInteractDistance()
		{
			return interactDistance;
		}

		public void Interact(BasePlayerCharacterController controller)
		{
			if (controller == null)
				return;

			if (manager == null)
				manager = FindObjectOfType<InteractionManager>();

			if (manager == null)
				return;

			manager.HandleInteractionRequest(
				controller.PlayingCharacterEntity,
				new WorldInteractionRequest
				{
					PlayerObjectId = controller.PlayingCharacterEntity.ObjectId,
					WorldObjectId = worldObjectId,
					InteractType = interactType
				});
		}

		#endregion
	}
}
