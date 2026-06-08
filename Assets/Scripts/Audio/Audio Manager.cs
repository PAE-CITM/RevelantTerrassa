using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource voiceSource;
    public AudioClip audioInicio;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }
}