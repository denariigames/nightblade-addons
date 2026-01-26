using UnityEngine;
using System.Collections.Generic;

namespace NightBlade
{
	/// <summary>
	/// Simple visual feedback for WorldInteraction hover.
	/// Changes renderer color on hover enter/exit.
	/// Debug / validation only.
	/// </summary>
	public sealed class WorldInteractionHoverColor : MonoBehaviour
	{
		[SerializeField] private Color hoverColor = Color.yellow;
		[Tooltip("Will use all renderers found if list not explicitly set")]
		[SerializeField] private Renderer[] renderers;

		private readonly Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();

		private void Awake()
		{
			if (renderers.Length == 0)
				renderers = GetComponentsInChildren<Renderer>(true);

			foreach (Renderer renderer in renderers)
			{
				if (renderer == null) continue;
				_originalMaterials[renderer] = (Material[])renderer.sharedMaterials.Clone();
			}
		}

		public void OnHoverEnter()
		{
			foreach (Renderer renderer in renderers)
			{
				if (renderer == null) continue;

				var mats = renderer.materials;
				for (int i = 0; i < mats.Length; i++)
				{
					mats[i].color = hoverColor;
				}

				renderer.materials = mats;
			}
		}

		public void OnHoverExit()
		{
			foreach (Renderer renderer in renderers)
			{
				if (renderer == null) continue;

				renderer.materials = _originalMaterials[renderer];
			}
		}
	}
}
