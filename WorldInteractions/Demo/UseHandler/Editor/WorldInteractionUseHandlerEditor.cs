using UnityEngine;
using UnityEditor;

namespace NightBlade
{
	[CustomEditor(typeof(WorldInteractionUseHandler))]
	public class WorldInteractionUseHandlerEditor : Editor
	{
		[CustomPropertyDrawer(typeof(WorldInteractableUseItem))]
		public class WorldInteractableUseItemDrawer : PropertyDrawer
		{
			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

				float totalWidth = position.width;
				float idWidth    = Mathf.Min(60, totalWidth * 0.2f);
				float amountWidth = Mathf.Min(50, totalWidth * 0.2f);
				float itemWidth   = totalWidth - idWidth - amountWidth - 10; // spacing

				Rect idRect     = new Rect(position) { width = idWidth };
				Rect itemRect   = new Rect(idRect)   { x = idRect.xMax + 5, width = itemWidth };
				Rect amountRect = new Rect(itemRect) { x = itemRect.xMax + 5, width = amountWidth };

				var worldId   = property.FindPropertyRelative("worldId");
				var item   = property.FindPropertyRelative("item");
				var amount    = property.FindPropertyRelative("amount");

				EditorGUI.PropertyField(idRect,     worldId,   GUIContent.none);
				EditorGUI.PropertyField(itemRect,   item,   GUIContent.none);
				EditorGUI.PropertyField(amountRect, amount,    GUIContent.none);
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawPropertiesExcluding(serializedObject, "useItems");

			EditorGUILayout.Space(12);
			EditorGUILayout.HelpBox("Enter the `World Id` set on the WorldInteractableTarget, select an `item` and enter `quantity` to give to player when used.", MessageType.None, true);

			SerializedProperty useItemsProp = serializedObject.FindProperty("useItems");

			if (useItemsProp == null)
			{
				EditorGUILayout.HelpBox("useItems property not found.", MessageType.Error);
				return;
			}

			bool expanded = EditorGUILayout.Foldout(useItemsProp.isExpanded, "Use Items");
			useItemsProp.isExpanded = expanded;

			if (expanded)
			{
				EditorGUI.indentLevel++;

				for (int i = 0; i < useItemsProp.arraySize; i++)
				{
					SerializedProperty element = useItemsProp.GetArrayElementAtIndex(i);

					Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(rect, element, GUIContent.none, true);
				}

				EditorGUI.indentLevel--;

				// Simple + / - size controls (or add ReorderableList later)
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Add Item"))
				{
					useItemsProp.arraySize++;
				}
				if (useItemsProp.arraySize > 0 && GUILayout.Button("Remove Last"))
				{
					useItemsProp.arraySize--;
				}
				EditorGUILayout.EndHorizontal();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}