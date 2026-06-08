using System.Threading.Tasks;
using UnityEngine;

// Attach to object you want to be able to toggle on and off
public class DelayedGameObjectToggle : MonoBehaviour
{
    public void DelayedToggle(int milliseconds)
    {
        AsyncDelayedToggle(milliseconds);
    }

    private async Task AsyncDelayedToggle(int milliseconds)
    {
        await Task.Delay(milliseconds);
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
