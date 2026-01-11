using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
//Section for displaying the eventSpeaker type on the inspector
[CustomPropertyDrawer(typeof(DialogueEvent.eventSpeaker))]
public class EventSpeakersDrawer : PropertyDrawer
{
    const int SPACING_X = 80;
    const int ELEMENT_WIDTH = 200;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty modifiableSide = property.FindPropertyRelative("modifiableSide");

        //Display title
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Determine element postions
        var sideRect = new Rect(position.x, position.y, ELEMENT_WIDTH, position.height);
        var nameRect = new Rect(position.x + SPACING_X, position.y, ELEMENT_WIDTH * 2, position.height);


        if (modifiableSide.boolValue == true)
        {
            //Show SIDE as a changeable field
            EditorGUI.PropertyField(sideRect, property.FindPropertyRelative("speakerSide"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("speakerName"), GUIContent.none);
        }
        else
        {
            //Show SIDE as a label
            SerializedProperty enumValue = property.FindPropertyRelative("speakerSide");
            EditorGUI.LabelField(sideRect, enumValue.enumDisplayNames[enumValue.enumValueIndex].ToString());
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("speakerName"), GUIContent.none);
        }
    }
}

[CustomPropertyDrawer(typeof(DialogueEvent.DialogueEntry))]
public class DialogueEntryDrawer : PropertyDrawer
{
    const int SPACING_X = 80;
    const int SPACING_Y = 20;
    const int ELEMENT_WIDTH = 200;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //Display title
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Determine element postions
        var firstRect = new Rect(position.x, position.y, ELEMENT_WIDTH, position.height);
        var secondRect = new Rect(position.x + SPACING_X, position.y, ELEMENT_WIDTH, position.height);
        var secondRow = new Rect(position.x, position.y + SPACING_Y, ELEMENT_WIDTH, position.height);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.PropertyField(firstRect, property.FindPropertyRelative("speaker"), GUIContent.none);
        EditorGUI.PropertyField(secondRect, property.FindPropertyRelative("speakerImage"), GUIContent.none);
        EditorGUI.PropertyField(secondRow, property.FindPropertyRelative("dialogueText"), GUIContent.none);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}
#endif