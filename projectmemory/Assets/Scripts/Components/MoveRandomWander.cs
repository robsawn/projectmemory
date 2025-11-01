using UnityEngine;

/// <summary>
/// Handles random wandering movement behavior.
/// Can be used on any entity that needs to wander.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MoveRandomWander : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;
    [SerializeField] private float targetReachThreshold = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool isWaiting;
    private float waitTimer;
    private bool isPaused;

    public bool IsMoving => !isWaiting && !isPaused && rb.linearVelocity.magnitude > 0.1f;
    public Vector2 MovementDirection => rb.linearVelocity.normalized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        startPosition = transform.position;
        ChooseNewTarget();
    }

    private void Update()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isWaiting) HandleWaiting();
        else HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        rb.linearVelocity = direction * moveSpeed;

        if (direction.x != 0) spriteRenderer.flipX = direction.x < 0;

        if (Vector2.Distance(currentPosition, targetPosition) < targetReachThreshold) StartWaiting();
    }

    private void HandleWaiting()
    {
        rb.linearVelocity = Vector2.zero;
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
        {
            isWaiting = false;
            ChooseNewTarget();
        }
    }

    private void ChooseNewTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + randomDirection;
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Pauses wandering behavior.
    /// Entity stops moving until Resume method called.
    /// </summary>
    public void Pause()
    {
        isPaused = true;
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Resumes wandering behavior after being paused.
    /// </summary>
    public void Resume()
    {
        isPaused = false;
        if (!isWaiting) ChooseNewTarget();
    }

    /// <summary>
    /// Sets a new wander center point
    /// </summary>
    /// <param name="newCenter"></param>
    public void SetWanderCenter(Vector2 newCenter)
    {
        startPosition = newCenter;
        ChooseNewTarget();
    }

    private void OnDrawGizmosSelected()
    {
        // visualize wander radius
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);

        // show target position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
        }
    }
}
