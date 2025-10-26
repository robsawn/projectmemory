//using System;
using System;
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
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float moveSpeed = 10.0f;

    [Flags] public enum direction
    {
        NONE    = 0,
        UP      = 1 << 1,
        DOWN    = 1 << 2,
        LEFT    = 1 << 3,
        RIGHT   = 1 << 4,
    }

    public direction worldViewLook = 0;

    InputAction moveAction;

    private void OnEnable()
    {
        //MARK: update as gamemaster is built
        moveSpeed = GameMaster._instance.player_moveSpeed;    
    }


    private void OnDisable()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 input_move = moveAction.ReadValue<Vector2>() * Time.fixedDeltaTime;

        //add vector to player position?
        //playerRB.AddForce(input_move * Time.fixedDeltaTime);
        gameObject.transform.position += new Vector3(input_move.x, input_move.y, 0);

        Vector2 currentVelocity = playerRB.linearVelocity ;
        worldViewLook = (input_move.x > 0 ? direction.RIGHT : (input_move.x < 0 ? direction.LEFT : direction.NONE)) | 
                        (input_move.y > 0 ? direction.UP    : (input_move.y < 0 ? direction.DOWN : direction.NONE));
    }
}
