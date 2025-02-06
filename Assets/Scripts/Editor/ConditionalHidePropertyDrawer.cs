using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Attributes {
	
	[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
	public class ConditionalHidePropertyDrawer : PropertyDrawer {
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			ConditionalHideAttribute conditionalHide = (ConditionalHideAttribute)attribute;
			bool enabled = GetConditionalHideAttributeResult(conditionalHide, property);

			bool wasEnabled = GUI.enabled;
			GUI.enabled = enabled;

			if (!conditionalHide.HideInInspector || enabled) {
				EditorGUI.PropertyField(position, property, label, true);
			}

			GUI.enabled = wasEnabled;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			ConditionalHideAttribute conditionalHide = (ConditionalHideAttribute)attribute;
			bool enabled = GetConditionalHideAttributeResult(conditionalHide, property);

			if (!conditionalHide.HideInInspector || enabled) {
				return EditorGUI.GetPropertyHeight(property, label);
			}
			return -EditorGUIUtility.standardVerticalSpacing;
		}

		// ReSharper disable Unity.PerformanceAnalysis
		private bool GetConditionalHideAttributeResult(ConditionalHideAttribute conditionalHide, SerializedProperty property) {
			SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionalHide.ConditionalSourceField);

			if (sourcePropertyValue == null) {
				Debug.LogWarning($"Cannot find property with name: {conditionalHide.ConditionalSourceField}");
				return true;
			}

			return sourcePropertyValue.boolValue;
		}
	}
}