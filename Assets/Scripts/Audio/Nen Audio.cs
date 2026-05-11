using UnityEngine;
using System.Collections;

public class NPCVoiceSequence : MonoBehaviour
{
    
    public AudioSource audioSource;

    public AudioClip firstClip;
    public AudioClip secondClip;
    public AudioClip thirdClip;

    private float delayBetweenClips = 9f;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        PlayClip(firstClip);

        StartCoroutine(PlaySequence1());
    }

    private IEnumerator PlaySequence1()
    {
        

        yield return new WaitForSeconds(delayBetweenClips);

        PlayClip(secondClip);

        yield return new WaitForSeconds(13);

        PlayClip(thirdClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}