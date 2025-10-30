using UnityEngine;

public class InteractableExample : MonoBehaviour, IInteractable
{
    //public string name => this.name;

    //public Transform transform => this.transform;

    //public GameObject gameObject => this.gameObject;

    public void Activateinteractable()
    {
        Debug.Log("[InteractableExample] Test interactable was activated");
    }

}
