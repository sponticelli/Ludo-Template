using UnityEditor;
using UnityEngine;
using Ludo.Attributes;

namespace Ludo.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Store the original GUI enabled state
            bool wasEnabled = GUI.enabled;

            // Disable GUI to make it read-only
            GUI.enabled = false;

            // Draw the property field as read-only
            EditorGUI.PropertyField(position, property, label, true);

            // Restore the original GUI enabled state
            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Return the default height for the property
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}