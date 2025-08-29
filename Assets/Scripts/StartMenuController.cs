using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Clicked Jugar!");
        SceneManager.LoadScene("GreyboxScene"); // Replace with your main scene name
    }
}
