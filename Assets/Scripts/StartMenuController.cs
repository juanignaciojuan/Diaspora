using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Clicked Jugar!");
        SceneManager.LoadScene("Infierno"); // Replace with your main scene name
    }
}
