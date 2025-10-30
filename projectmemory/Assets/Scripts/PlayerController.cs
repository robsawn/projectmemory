//using System;
using System;
using System.Collections.Generic;
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

    private Dictionary<int, GameObject> nearbyInteractables = new Dictionary<int, GameObject>();
    private bool nearInteractable = false;

    private Vector2 inputMove = Vector2.zero;
 
    InputAction moveAction;
    InputAction interactAction;

    private void OnEnable()
    {
        InputSystem.actions.Enable();
        //MARK: update as gamemaster is built
        moveSpeed = GameMaster._instance.player_moveSpeed;    
    }


    private void OnDisable()
    {   
        //disable controls when player controller not longer active
        InputSystem.actions.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        inputMove = moveAction.ReadValue<Vector2>() * moveSpeed * Time.fixedDeltaTime;

        if(nearInteractable && interactAction.WasPressedThisFrame())
        {
            //activate interact on selected interactable
            Debug.Log($"[PlayerController]: Attempting to interact");
        }

        //add vector to player position?
        //playerRB.AddForce(input_move * Time.fixedDeltaTime);
        gameObject.transform.position += new Vector3(inputMove.x, inputMove.y, 0);

        Vector2 currentVelocity = playerRB.linearVelocity;
        worldViewLook = (inputMove.x > 0 ? direction.RIGHT : (inputMove.x < 0 ? direction.LEFT : direction.NONE)) |
                        (inputMove.y > 0 ? direction.UP : (inputMove.y < 0 ? direction.DOWN : direction.NONE));
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var checkTest = other.gameObject.GetComponent<InteractableBase>();
        if (checkTest != null)
        {
            int tempID = other.gameObject.GetInstanceID();
            if (!nearbyInteractables.ContainsKey(tempID))
            {
                try
                {
                    nearbyInteractables.Add(other.gameObject.GetInstanceID(), other.gameObject);
                    nearInteractable = true;
                    Debug.Log($"[PlayerController]: detected nearby interactable. ID: {other.gameObject.GetInstanceID()}");
                }
                catch (Exception)
                {

                    throw;
                }
                other.gameObject.GetInstanceID();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var checkTest = other.gameObject.GetComponent<InteractableBase>();
        if (checkTest != null && nearbyInteractables.ContainsKey(checkTest.GetInstanceID()))
        {
            try
            {
                int tempID = other.gameObject.GetInstanceID();
                nearbyInteractables.Remove(tempID);
                Debug.Log($"[PlayerController]:Removed interactable from list. ID: {tempID}");
                if(nearbyInteractables.Count == 0)
                {
                    nearInteractable = false;
                    Debug.Log($"[PlayerController]:No longer near interactables. nearInteractable: {nearInteractable}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
