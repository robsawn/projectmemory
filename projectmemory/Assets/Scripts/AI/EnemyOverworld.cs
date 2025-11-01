using UnityEngine;

/// <summary>
/// Enemy agent that wanders overworld and triggers battles when interacted with.
/// 
/// Uses:
///     MoveRandomWander for movement
/// Extends:
///     Interactable for interaction logic
///     
/// Will need later updates
/// </summary>
[RequireComponent(typeof(MoveRandomWander))]
public class EnemyOverworld : Interactable
{
    [Header("Enemy Behavior")]
    [SerializeField] private bool stopMovementDuringInteraction = true;

    private MoveRandomWander movement;

    private void Awake() { movement = GetComponent<MoveRandomWander>(); }

    /// <summary>
    /// Called when player interacts with this enemy
    /// Battle starts
    /// </summary>
    /// <param name="player"></param>
    public override void OnInteract(GameObject player)
    {
        Debug.Log($"{gameObject.name} interacting with {player.name}");

        if (stopMovementDuringInteraction && movement != null) movement.Pause();

        // battle logic goes here
        // ex.: BattleManager.Instance.StartBattle(this, player) kind of thing
    }

    /// <summary>
    /// Called when player enters interaction range
    /// Use to show visual indicators, play sounds, etc
    /// </summary>
    /// <param name="player"></param>
    protected override void OnPlayerEnterRange(Transform player)
    {
        base.OnPlayerEnterRange(player);

        // optionally, we can show exclamations, play alert sounds, whatever we end up doing
    }

    /// <summary>
    /// Called when player exits interaction range
    /// </summary>
    protected override void OnPlayerExitRange()
    {
        base.OnPlayerExitRange();

        // optionally, stop/hide whatever feedback we had
    }

    /// <summary>
    /// Call when wandering can resume 
    /// Ex.: battle is run away from, and enemy wasn't defeated 
    /// </summary>
    public void ResumeWandering() { if (movement != null) movement.Resume(); }

    /// <summary>
    /// Get reference to movement component as needed
    /// </summary>
    /// <returns></returns>
    public MoveRandomWander GetMovement() => movement;
}
