using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("Required Objects")]
    [SerializeField] private TMP_Text textName_Left;
    [SerializeField] private TMP_Text textSpeech_Left;
    [SerializeField] private TMP_Text textName_Right;
    [SerializeField] private TMP_Text textSpeech_Right;
    [SerializeField] private Image imgLeft_1;
    [SerializeField] private Image imgLeft_2;
    [SerializeField] private Image imgLeft_3;
    [SerializeField] private Image imgRight_1;
    [SerializeField] private Image imgRight_2;
    [SerializeField] private Image imgRight_3;

    [SerializeField] private AudioClip textSound;

    [Header("Settings")]
    [SerializeField] private Color colourActiveSpeaker;
    [SerializeField] private Color colourInactiveSpeaker;
    [SerializeField] private bool playSoundAtEveryLetter = true;
    [SerializeField] private float printDelayBetweenChar = 0.2f;

    [Header("Testing")]
    [SerializeField] private DialogueEvent targetEvent;
    [SerializeField] private Coroutine currentLine_Displaying = null;

    public static DialogueController _instance { get; private set; }
    private AudioSource controllerAudioSource;
    private DialogueEvent currentEvent = null;
    private bool finishedLine = true;
    private void Awake()
    {
        //Check instance to make sure there's only one
        if(_instance!=null && _instance!= this)
        {
            Destroy(this.gameObject);
        }
        else
        {   
            controllerAudioSource = GetComponent<AudioSource>();
            _instance = this;
        }

        HideUI();
        //Link to button actions here
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if(!finishedLine)
            {
                finishedLine = true;
            }
            else if(!DisplayNextEntry())
            {
                HideUI();
            }
        }
    }

    //Run the event set in TargetEvent
    public bool RunTargetEvent()
    {
        if (targetEvent != null)
        {
            finishedLine = false;
            return RunEvent(targetEvent);
        }
        else
        {
            Debug.LogError("ERR[DialogueController]: Attempted to run target dialogue event when none set");
            return false;
        }
    }

    public bool RunEvent(DialogueEvent target)
    {
        ShowUI();
        currentEvent = target;
        //Setup scene
        return DisplayNextEntry();
    }

    /// <summary>
    /// Get the next dialogue event and display it.
    /// Returns false if there is no more events to display
    /// </summary>
    /// <returns></returns>
    public bool DisplayNextEntry()
    {
        DialogueEvent.DialogueEntry next = currentEvent.GetNextEntry();
        finishedLine = false;

        //If there is a speaking side, then the dialogue is still ongoing
        if (next.speaker != DialogueEvent.SIDE.NONE)
        {
            //TODO: checks for if incorrect object is set?
            if (next.speaker == DialogueEvent.SIDE.LEFT1 ||
                next.speaker == DialogueEvent.SIDE.LEFT2 ||
                next.speaker == DialogueEvent.SIDE.LEFT3)
            {
                textSpeech_Left.transform.parent.gameObject.SetActive(true);
                currentLine_Displaying = StartCoroutine( PrintDialogueText(next.dialogueText,textSpeech_Left) );
                textName_Left.text = currentEvent.GetSpeakerName(next.speaker);
                imgLeft_1.sprite = next.speakerImage;
                imgLeft_1.color = colourActiveSpeaker;

                textSpeech_Right.transform.parent.gameObject.SetActive(false);
                imgRight_1.color = colourInactiveSpeaker;
                textName_Right.text = string.Empty;
                textSpeech_Right.text = string.Empty;
            }
            else if (next.speaker == DialogueEvent.SIDE.RIGHT1 ||
                     next.speaker == DialogueEvent.SIDE.RIGHT2 ||
                     next.speaker == DialogueEvent.SIDE.RIGHT3)
            {
                textSpeech_Right.transform.parent.gameObject.SetActive(true);
                currentLine_Displaying = StartCoroutine(PrintDialogueText(next.dialogueText, textSpeech_Right));
                textName_Right.text = currentEvent.GetSpeakerName(next.speaker);
                imgRight_1.sprite = next.speakerImage;
                imgRight_1.color = colourActiveSpeaker;

                textSpeech_Left.transform.parent.gameObject.SetActive(false);
                imgLeft_1.color = colourInactiveSpeaker;
                textName_Left.text = string.Empty;
                textSpeech_Left.text = string.Empty;
            } 
            return true;
        }
        else
        //If the event return has no side,
        //then assume it has ended
        {
            return false;
        }
    }

    private void OnDisable()
    {
        //Unlink from actions here
    }

    private void ShowUI()
    {
        //textSpeech_Left.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
        //textSpeech_Left.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator PrintDialogueText(string text, TMP_Text display)
    {
        string lineToDisplay = text;
        TMP_Text textToDisplay = display;

        for(int index = 0; index <= text.Length; index++)
        {
            if (!finishedLine)
            {
                textToDisplay.text = lineToDisplay.Substring(0, index);
                //Play beep at every letter is required
                if(playSoundAtEveryLetter && textSound != null)
                {
                    controllerAudioSource.PlayOneShot(textSound);
                }
                yield return new WaitForSeconds(printDelayBetweenChar); 
            }
            else
            {
                textToDisplay.text = lineToDisplay;
                break;
            }
        }

        //Play beep at end of line if not set to play at every letter
        if (!playSoundAtEveryLetter && textSound != null)
        {
            controllerAudioSource.PlayOneShot(textSound);
        }

        finishedLine = true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueController))]
public class DialogueControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //Only show the Run button if we're in Play mode
        if(Application.isPlaying)
        {
            if(GUILayout.Button("RunDilalogueEvent"))
            {
                DialogueController._instance.RunTargetEvent();
            }
        }
    }
}
#endif