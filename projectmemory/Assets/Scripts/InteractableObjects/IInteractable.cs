using UnityEngine;

public interface IInteractable
{
    //#region MonobehaviourProperties
    //public string name { get; }
    //public Transform transform { get; }
    //public GameObject gameObject { get; }

    //#endregion
    //public enum InteractType
    //{
    //    NONE,
    //    TEST,
    //    DIALOGUE,
    //    ADDITEM,
    //    REMOVEITEM,
    //}

    //public InteractType type = InteractType.NONE;

    //virtual public InteractType GetInteractType()
    //{
    //    return type;
    //}

    public void Activateinteractable();
}
