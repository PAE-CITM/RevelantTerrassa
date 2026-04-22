using UnityEngine;

public class MenuInicial : MonoBehaviour
{
    public GameObject canvasMenu;
    public void StartExperience()
    {
        canvasMenu.SetActive(false);

        Debug.Log("Starting Onboarding...");
    }

    public void ExitGame()
    {
        Debug.Log("Exit app...");
        Application.Quit();
    }
}