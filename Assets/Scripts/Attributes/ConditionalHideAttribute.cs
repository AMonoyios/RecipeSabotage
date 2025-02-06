using UnityEngine;

namespace Attributes {
	
	public class ConditionalHideAttribute : PropertyAttribute {
		public string ConditionalSourceField { get; set; }
		public bool HideInInspector { get; set; }

		public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false) {
			ConditionalSourceField = conditionalSourceField;
			HideInInspector = hideInInspector;
		}
	}
}