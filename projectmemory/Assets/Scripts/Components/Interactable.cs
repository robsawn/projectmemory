using UnityEngine;

/// <summary>
/// Base class for any interactable entity
/// Extend this class to create different interaction types (enemies, NPCs, chests, etc.)
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] private float interactRadius = 1.5f;

    private void Awake()
    {
        CreateTrigger();
        OnStart();
    }

    /// <summary>
    /// Optional override for child classes that need Start logic
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// Create a trigger collider for interact logic
    /// </summary>
    private void CreateTrigger()
    {
        // sanity check to see if trigger collider is already present
        // if one is present, skip
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
            if (col.isTrigger) return;

        // no trigger found, create one
        CircleCollider2D trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = interactRadius;

        Debug.Log($"Trigger collider created for {gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { if (IsPlayer(collision)) OnPlayerEnterRange(collision.transform); }

    private void OnTriggerExit2D(Collider2D collision)
    { if (IsPlayer(collision)) OnPlayerExitRange(); }

    private bool IsPlayer(Collider2D collision)
    {
        int layerMask = 1 << collision.gameObject.layer;
        return collision.CompareTag("Player") && (layerMask & playerLayer) != 0;
    }

    /// <summary>
    /// Called when player presses interact button.
    /// Override to implement interactions
    /// </summary>
    /// <param name="player"></param>
    public abstract void OnInteract(GameObject player);

    /// <summary>
    /// Called when player enters range.
    /// Override to show prompts, etc.
    /// </summary>
    /// <param name="player"></param>
    protected virtual void OnPlayerEnterRange(Transform player) { }

    /// <summary>
    /// Called when player exits range
    /// Override to hide prompts, etc
    /// </summary>
    protected virtual void OnPlayerExitRange() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger && col is CircleCollider2D circle)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
                return;
            }
        }

        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
