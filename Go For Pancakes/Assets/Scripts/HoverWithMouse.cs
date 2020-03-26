using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverWithMouse : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnMouseEnter()
    {
        audioSource.PlayOneShot(clips[0]);
    }
    public void OnMouseClick()
    {
        audioSource.PlayOneShot(clips[1]);
    }
}
