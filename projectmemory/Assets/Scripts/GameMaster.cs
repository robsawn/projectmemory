//-----------------------------------
// Singleton for managing player stats,
// to ease transferring between scenes
// and loading/saving data
//
//-----------------------------------

using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster _instance { get; private set; } = null;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    //Possible global fucntions
    public void PauseGame()
    {

    }

    public void FreezeOvermap()
    {

    }

    public void TransitionToBattleScreen()
    {

    }

    public void TransitionToBossScreen()
    {

    }
}
