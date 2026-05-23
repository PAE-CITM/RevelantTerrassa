using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using OnBoarding;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        LoadScene();
    }

    async Task LoadScene()
    {
        await ScreenFader.Instance.FadeAsync(0.0f, 1.0f, 3);
        SceneManager.LoadScene("OnBoarding");
    }

    public void QuitGame()
    {
        QuitGameAsync();
    }

    async Task QuitGameAsync()
    {
        await ScreenFader.Instance.FadeAsync(0.0f, 1.0f, 3);
        Debug.Log("Salir del juego");
        Application.Quit(); // Ignored in editor
    }
}