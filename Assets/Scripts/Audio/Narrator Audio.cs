using UnityEngine;
using System.Collections;

public class narratorAudio : MonoBehaviour
{
    
    public AudioSource audioSource;

    public AudioClip firstClip;

    [SerializeField] private float firstClipDelay = 10f;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlayFirstClipWithDelay());
    }

    private IEnumerator PlayFirstClipWithDelay()
    {
        yield return new WaitForSeconds(firstClipDelay);
        PlayClip(firstClip);
    }

   
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        
    }
}