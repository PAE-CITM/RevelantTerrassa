using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return FadeManager.Instance.FadeOut();
        SceneManager.LoadScene("OnBoarding");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}