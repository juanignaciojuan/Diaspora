using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool hasKey = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
}
