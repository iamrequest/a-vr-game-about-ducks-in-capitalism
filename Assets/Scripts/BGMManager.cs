using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour {
    private AudioSource audioSource;
    public AudioClip intro, loop;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(intro);
        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.PlayDelayed(intro.length);
    }
}
