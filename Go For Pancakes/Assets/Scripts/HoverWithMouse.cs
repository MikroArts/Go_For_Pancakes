using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverWithMouse : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnMouseEnter()
    {
        audioSource.PlayOneShot(clip);
    }
}
