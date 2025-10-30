//using System;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        NONE    = 0,
        UP      = 1 << 1,
        DOWN    = 1 << 2,
        LEFT    = 1 << 3,
        RIGHT   = 1 << 4,
    }

    public direction worldViewLook = 0;

    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float moveSpeed = 10.0f;

    private InputActionMap playerControlMap;
    private Dictionary<int, GameObject> nearbyInteractables = new Dictionary<int, GameObject>();
    private bool nearInteractable = false;
    private int interactableIndex = -1;

    private Vector2 inputMove = Vector2.zero;
 
    InputAction moveAction;
    InputAction interactAction;
    InputAction prevAction;
    InputAction nextAction;

    private void OnEnable()
    {
        playerControlMap.Enable();
        //MARK: update as gamemaster is built
        //Refreash skill/statuses/stats/equipped stuff
        moveSpeed = GameMaster._instance.player_moveSpeed;    
    }


    private void OnDisable()
    {   
        //disable controls when player controller not longer active        
       playerControlMap.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        interactAction = InputSystem.actions.FindAction("Interact");
        playerControlMap = InputSystem.actions.FindActionMap("Player");
        prevAction = InputSystem.actions.FindAction("Previous");
        nextAction = InputSystem.actions.FindAction("Next");
    }

    // Update is called once per frame
    void Update()
    {

        #region actionKeys
        if (nearInteractable && interactAction.WasPressedThisFrame())
        {
            //activate interact on selected interactable
            Debug.Log($"[PlayerController]: Attempting to interact");
        }
        else if(nearbyInteractables.Count > 0)
        {
            if(prevAction.WasPerformedThisFrame())
            {
                Debug.Log($"[PlayerController]: Switch target to previous nearby interactable");
            }
            else if(nextAction.WasPressedThisFrame())
            {
                Debug.Log($"[PlayerController]: Switch target to next nearby interactable");
            }
        }
        #endregion

            #region movement
            inputMove = moveAction.ReadValue<Vector2>() * moveSpeed * Time.fixedDeltaTime;

        //add vector to player position?
        //playerRB.AddForce(input_move * Time.fixedDeltaTime);
        gameObject.transform.position += new Vector3(inputMove.x, inputMove.y, 0);

        Vector2 currentVelocity = playerRB.linearVelocity;
        worldViewLook = (inputMove.x > 0 ? direction.RIGHT : (inputMove.x < 0 ? direction.LEFT : direction.NONE)) |
                        (inputMove.y > 0 ? direction.UP : (inputMove.y < 0 ? direction.DOWN : direction.NONE));

        #endregion
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
                nearbyInteractables.Add(other.gameObject.GetInstanceID(), other.gameObject);
                nearInteractable = true;
                Debug.Log($"[PlayerController]: detected nearby interactable. ID: {other.gameObject.GetInstanceID()}");
                if(nearbyInteractables.Count == 1)
                {   
                    //Highlight nearest instead? Do we still check if it's the first one?
                    HighlightNearbyInteractable(0);
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
            if(nearbyInteractables.ContainsKey(other.gameObject.GetInstanceID()))
            {
                nearbyInteractables.Remove(tempID);
                Debug.Log($"[PlayerController]:Removed interactable from list. ID: {tempID}");

                if(nearbyInteractables.Count == 0)
                {
                    nearInteractable = false;
                    Debug.Log($"[PlayerController]:No longer near interactables. nearInteractable: {nearInteractable}");
                }
            }
        }
    }

    private void HighlightNearbyInteractable(int index)
    {
        Debug.Log($"[PlayerController]: Highlighting interactable at: index: {index}; ID: {nearbyInteractables.ElementAt(index).Value}");
        interactableIndex = index;
    }
}
