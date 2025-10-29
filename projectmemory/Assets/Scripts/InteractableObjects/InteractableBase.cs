using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public enum InteractType
    {
        NONE,
        TEST,
        DIALOGUE,
        ADDITEM,
        REMOVEITEM,
    }

    [SerializeField] private InteractType type = InteractType.NONE;

    virtual public InteractType GetInteractType()
    {
        return type;
    }
}
