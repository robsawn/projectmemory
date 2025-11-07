using UnityEngine;

public class InteractableExample : MonoBehaviour, IInteractable
{
    public void ActivateInteractable()
    {
        Debug.Log($"[InteractableExample] {name} was activated");
    }

}
