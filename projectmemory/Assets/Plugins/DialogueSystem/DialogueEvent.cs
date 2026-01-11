using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEvent : MonoBehaviour
{
    #region helperClasses
    [System.Serializable]
    public struct DialogueEntry
    {
        public SIDE speaker;
        public Sprite speakerImage;
        public string dialogueText;

        public DialogueEntry(DialogueEntry input)
        { 
            speaker = input.speaker;
            dialogueText = input.dialogueText;
            speakerImage = input.speakerImage;
        }
    }

    [System.Serializable]
    public struct eventSpeaker
    {
        public SIDE speakerSide;
        public string speakerName;
        public bool modifiableSide;

        public eventSpeaker(eventSpeaker input)
        {
            speakerSide = input.speakerSide;
            speakerName = input.speakerName;
            modifiableSide = input.modifiableSide;
        }
    }

    public enum SIDE
    {
        NONE,
        LEFT1,
        LEFT2,
        LEFT3,
        RIGHT1,
        RIGHT2,
        RIGHT3,
    }

    /// <summary>
    /// Something to track what speakers are involved in the 
    /// event. A necessary alternative to using dictionary 
    /// to allow setting through the inspector
    /// </summary>
    [Serializable]
    public class EventSpeakers
    {
        public eventSpeaker speaker_L1 = new eventSpeaker();
        public eventSpeaker speaker_L2 = new eventSpeaker();
        public eventSpeaker speaker_L3 = new eventSpeaker();

        public eventSpeaker speaker_R1 = new eventSpeaker();
        public eventSpeaker speaker_R2 = new eventSpeaker();
        public eventSpeaker speaker_R3 = new eventSpeaker();

        public EventSpeakers()
        {
            speaker_L1.speakerSide = SIDE.LEFT1;
            speaker_L2.speakerSide = SIDE.LEFT2;
            speaker_L3.speakerSide = SIDE.LEFT3;
            speaker_L1.modifiableSide = true;
            speaker_L2.modifiableSide = true;
            speaker_L3.modifiableSide = true;

            speaker_R1.speakerSide = SIDE.RIGHT1;
            speaker_R2.speakerSide = SIDE.RIGHT2;
            speaker_R3.speakerSide = SIDE.RIGHT3;
            speaker_R1.modifiableSide = true;
            speaker_R2.modifiableSide = true;
            speaker_R3.modifiableSide = true;
        }

        public string GetSpeakerName(SIDE speaker)
        {
            switch (speaker) 
            {
                case    SIDE.LEFT1:
                {
                    return speaker_L1.speakerName;
                }
                case SIDE.LEFT2:
                {
                    return speaker_L2.speakerName;
                }
                case SIDE.LEFT3:
                {
                    return speaker_L3.speakerName;
                }
                case SIDE.RIGHT1:
                {
                    return speaker_R1.speakerName;
                }
                case SIDE.RIGHT2:
                {
                    return speaker_R2.speakerName;
                }
                case SIDE.RIGHT3:
                {
                    return speaker_R3.speakerName;
                }
                default:
                {
                    return string.Empty;
                }
            }
        }
    }
    #endregion

    #region MainBody
    [Header("Event Settings")]
    [SerializeField] private EventSpeakers speakerNames = new EventSpeakers();
    [SerializeField] private List<DialogueEntry> eventDialogue = new List<DialogueEntry>();

    //[Header("Event Variables")]
    private int count = 0;
    public bool isComplete { get; private set; }

    private void Awake()
    {
        isComplete = false;
        count = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetEvent()
    {
        isComplete = false;
        count = 0;
    }

    public DialogueEntry GetNextEntry() 
    {
        //If we've gone through all the text, return an entry with None as side
        if(count< eventDialogue.Count)
        {
            //Create a copy to avoid accidentally changing data
            DialogueEntry copy = new DialogueEntry(eventDialogue[count]);
            count++;
            return copy;
        }
        else
        {
            DialogueEntry output = new DialogueEntry();
            output.speaker = SIDE.NONE;
            count = 0;
            return output;
        }
    }

    /// <summary>
    /// Get the name associated with the side in this 
    /// dialogue event
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    public string GetSpeakerName(SIDE side)
    {
        return speakerNames.GetSpeakerName(side);
    }
    #endregion
}

//#if UNITY_EDITOR
////Section for displaying the eventSpeaker type on the inspector
//[CustomPropertyDrawer(typeof(DialogueEvent.eventSpeaker))]
//public class EventSpeakersDrawer : PropertyDrawer
//{
//    const int SPACING_X = 80;
//    const int SPACING_Y = 20;
//    const int ELEMENT_WIDTH = 200;

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);

//        SerializedProperty modifiableSide = property.FindPropertyRelative("modifiableSide");

//        //Display title
//        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        //Determine element postions
//        var sideRect = new Rect(position.x, position.y, ELEMENT_WIDTH, position.height);
//        var nameRect = new Rect(position.x + SPACING_X, position.y, ELEMENT_WIDTH * 2, position.height);


//        if (modifiableSide.boolValue == true)
//        {
//            //Show SIDE as a changeable field
//            EditorGUI.PropertyField(sideRect, property.FindPropertyRelative("speakerSide"), GUIContent.none);
//            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("speakerName"), GUIContent.none);
//        }
//        else
//        {
//            //Show SIDE as a label
//            SerializedProperty enumValue = property.FindPropertyRelative("speakerSide");
//            EditorGUI.LabelField(sideRect, enumValue.enumDisplayNames[enumValue.enumValueIndex].ToString());
//            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("speakerName"), GUIContent.none);
//        }
//    }
//}
//#endif
