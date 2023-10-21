using UnityEngine;

public class IntroUI : MonoBehaviour
{
    public void StartGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level01");
    }

    public void ExitGame(){
        Application.Quit();
    }
}
