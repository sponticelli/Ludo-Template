using UnityEditor;
using UnityEngine;
using Ludo.Attributes;
using System.Reflection;

namespace Ludo.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public class ConditionalAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalAttribute conditionalAttribute = (ConditionalAttribute)attribute;
            
            // Check if the condition is met
            bool shouldShow = EvaluateCondition(property, conditionalAttribute);
            
            if (shouldShow)
            {
                // Draw the property normally if condition is met
                EditorGUI.PropertyField(position, property, label, true);
            }
            else if (!conditionalAttribute.HideInInspector)
            {
                // Draw the property as disabled if HideInInspector is false
                bool wasEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = wasEnabled;
            }
            // If HideInInspector is true and condition is not met, don't draw anything
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalAttribute conditionalAttribute = (ConditionalAttribute)attribute;
            
            // Check if the condition is met
            bool shouldShow = EvaluateCondition(property, conditionalAttribute);
            
            if (shouldShow || !conditionalAttribute.HideInInspector)
            {
                // Return normal height if showing the property
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                // Return 0 height if hiding the property
                return 0f;
            }
        }
        
        private bool EvaluateCondition(SerializedProperty property, ConditionalAttribute conditionalAttribute)
        {
            // Handle single variable condition
            if (!string.IsNullOrEmpty(conditionalAttribute.VariableName))
            {
                return EvaluateSingleCondition(property, conditionalAttribute.VariableName, conditionalAttribute.IsTrue);
            }
            
            // Handle pair condition
            if (conditionalAttribute.Pair.VariableA != null && conditionalAttribute.Pair.VariableB != null)
            {
                bool conditionA = EvaluateSingleCondition(property, conditionalAttribute.Pair.VariableA, conditionalAttribute.Pair.ATrue);
                bool conditionB = EvaluateSingleCondition(property, conditionalAttribute.Pair.VariableB, conditionalAttribute.Pair.BTrue);
                
                switch (conditionalAttribute.Pair.Operators)
                {
                    case LogicOperators.AND:
                        return conditionA && conditionB;
                    case LogicOperators.OR:
                        return conditionA || conditionB;
                    default:
                        return conditionA && conditionB;
                }
            }
            
            // Default to true if no valid condition is found
            return true;
        }
        
        private bool EvaluateSingleCondition(SerializedProperty property, string variableName, bool expectedValue)
        {
            // Find the target property in the same object
            SerializedProperty targetProperty = property.serializedObject.FindProperty(variableName);
            
            if (targetProperty == null)
            {
                // If property not found, try to find it as a relative property
                string propertyPath = property.propertyPath;
                string basePath = "";
                
                // Handle nested properties (e.g., array elements, nested objects)
                int lastDotIndex = propertyPath.LastIndexOf('.');
                if (lastDotIndex >= 0)
                {
                    basePath = propertyPath.Substring(0, lastDotIndex + 1);
                    targetProperty = property.serializedObject.FindProperty(basePath + variableName);
                }
                
                if (targetProperty == null)
                {
                    Debug.LogWarning($"ConditionalAttribute: Property '{variableName}' not found for conditional evaluation.");
                    return true; // Default to showing the property if condition property is not found
                }
            }
            
            // Evaluate the condition based on property type
            bool actualValue = GetBooleanValue(targetProperty);
            return actualValue == expectedValue;
        }
        
        private bool GetBooleanValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                    
                case SerializedPropertyType.Integer:
                    return property.intValue != 0;
                    
                case SerializedPropertyType.Float:
                    return property.floatValue != 0f;
                    
                case SerializedPropertyType.String:
                    return !string.IsNullOrEmpty(property.stringValue);
                    
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null;
                    
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex != 0;
                    
                default:
                    // For other types, consider them true if they exist
                    return true;
            }
        }
    }
}
