using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif



public class PlayerController : MonoBehaviour
{

    [Flags] public enum direction
    {
        NONE = 0,
        UP = 1 << 1,
        DOWN = 1 << 2,
        LEFT = 1 << 3,
        RIGHT = 1 << 4,
    }

    public direction worldViewLook = 0;

    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private GameObject interactIconObject;

    private InputActionMap playerControlMap;
    private Dictionary<int, GameObject> nearbyInteractables = new Dictionary<int, GameObject>();
    private bool nearInteractable = false;
    private int interactableIndex = -1;

    private Vector2 inputMove = Vector2.zero;
    private bool enableControls = true;

    InputAction moveAction;
    InputAction interactAction;
    InputAction prevAction;
    InputAction nextAction;

    private void OnEnable()
    {

    }


    private void OnDisable()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerControlMap = InputSystem.actions.FindActionMap("Player");
        moveAction      = InputSystem.actions.FindAction("Move");
        interactAction  = InputSystem.actions.FindAction("Interact");
        prevAction      = InputSystem.actions.FindAction("Previous");
        nextAction      = InputSystem.actions.FindAction("Next");

        Debug.Assert(moveAction != null     , "[PlayerController]: No input action named 'Move'");
        Debug.Assert(interactAction != null , "[PlayerController]: No input action named 'Interact'");
        Debug.Assert(prevAction != null     , "[PlayerController]: No input action named 'Previous'");
        Debug.Assert(nextAction != null     , "[PlayerController]: No input action named 'Next'");

        Debug.Assert(interactIconObject!=null ,"[PlayerController]: interact icon object not set.");

        //Temporary workaround to avoid repeated errors if inputactions were changed and this wasn't updated to match
       if((moveAction == null) || (interactAction == null) || (prevAction == null) || (nextAction == null))
       {
            enableControls = false;
            Debug.LogError("[PlayerController]: player input will be ignored until script is updated to match input action settings");
       }
        

        HighlightNearbyInteractable(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableControls)
        {
            if (nearInteractable && interactAction.WasPressedThisFrame())
            {
                //activate interact on selected interactable
                nearbyInteractables.ElementAt(interactableIndex).Value.GetComponent<IInteractable>().ActivateInteractable();
            }
            else if (nearbyInteractables.Count > 0)
            {
                if (prevAction.WasPressedThisFrame())
                {
                    Debug.Log($"[PlayerController]: Switch target to previous nearby interactable");
                    interactableIndex--;
                    if (interactableIndex < 0)
                    {
                        interactableIndex = nearbyInteractables.Count - 1;
                    }
                    HighlightNearbyInteractable();
                }
                else if (nextAction.WasPressedThisFrame())
                {
                    Debug.Log($"[PlayerController]: Switch target to next nearby interactable");
                    interactableIndex++;
                    if (interactableIndex >= nearbyInteractables.Count)
                    {
                        interactableIndex = 0;
                    }
                    HighlightNearbyInteractable();
                }
            }

            inputMove = moveAction.ReadValue<Vector2>() * moveSpeed * Time.fixedDeltaTime;

            //add vector to player position?
            //playerRB.AddForce(inputMove);
            gameObject.transform.position += new Vector3(inputMove.x, inputMove.y, 0);

            //Vector2 currentVelocity = playerRB.linearVelocity;
            worldViewLook = (inputMove.x > 0 ? direction.RIGHT : (inputMove.x < 0 ? direction.LEFT : direction.NONE)) |
                            (inputMove.y > 0 ? direction.UP : (inputMove.y < 0 ? direction.DOWN : direction.NONE)); 
        }
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var checkTest = other.gameObject.GetComponent<IInteractable>();
        if (checkTest != null)
        {
            int tempID = other.gameObject.GetInstanceID();
            if (!nearbyInteractables.ContainsKey(tempID))
            {
                //TODO: sort as it's added? sort by x position?
                nearbyInteractables.Add(other.gameObject.GetInstanceID(), other.gameObject);
                nearInteractable = true;
                Debug.Log($"[PlayerController]: Interactable added to list. ID: {other.gameObject.GetInstanceID()}");
                if (nearbyInteractables.Count == 1)
                {
                    interactableIndex = 0;
                    HighlightNearbyInteractable();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var checkTest = other.gameObject.GetComponent<IInteractable>();
        if (checkTest != null)
        {
            int tempID = other.gameObject.GetInstanceID();
            if (nearbyInteractables.ContainsKey(tempID))
            {
                nearbyInteractables.Remove(tempID);

                if (nearbyInteractables.Count == 0)
                {
                    nearInteractable = false;
                    interactableIndex = -1;
                    Debug.Log($"[PlayerController]:Will no longer be near interactables. nearInteractable: {nearInteractable}");
                    HighlightNearbyInteractable(false);
                }
                else
                {
                    //if interactable index no longer valid, change it
                    if ( interactableIndex>=nearbyInteractables.Count )
                    {
                        interactableIndex++;
                        if (interactableIndex >= nearbyInteractables.Count)
                        {
                            interactableIndex = 0;
                        }
                    }

                    HighlightNearbyInteractable();
                }
                Debug.Log($"[PlayerController]:Removed interactable from list. ID: {tempID}");
            }
        }
    }

    /// <summary>
    /// Highlight selected interactable by moving icon to said interactable and
    /// enabling it. If nearInteractables is false, will function as if interactablesExist set to false
    /// </summary>
    /// <param name="interactablesExist"> if set to false, moves icon back to player and disable</param>
    private void HighlightNearbyInteractable(bool interactablesExist = true)
    {
        if (!interactablesExist || !nearInteractable )
        {
            //Return interact icon back to player and disable
            interactIconObject.transform.SetParent(gameObject.transform, false);
            interactIconObject.SetActive(false);            
        }
        else if(interactableIndex >= nearbyInteractables.Count)
        {
            //This must not happen. Reset if it does
            Debug.LogError($"[PlayerController]: Attempted to highlighting interactable at: index: {interactableIndex} when nearbyInteractables only has {nearbyInteractables.Count} entries");
            nearbyInteractables.Clear();
            nearInteractable = false;
            interactIconObject.transform.SetParent(gameObject.transform, false);
            interactIconObject.SetActive(false);
        }
        else
        {
            interactIconObject.SetActive(true);
            interactIconObject.transform.SetParent(nearbyInteractables.ElementAt(interactableIndex).Value.transform, false);
            Debug.Log($"[PlayerController]: Highlighting interactable at: index: {interactableIndex}; ID: {nearbyInteractables.ElementAt(interactableIndex).Key}");
        }
    }
}
