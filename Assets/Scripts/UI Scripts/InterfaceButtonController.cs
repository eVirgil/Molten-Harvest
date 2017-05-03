using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InterfaceButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{
    private AudioSource pointerDownSound;
    private AudioSource pointerEnterSound;
    private AudioSource pointerUpSound;
    public bool pointerWithinBounds;

    void Start()
    {
        initializeAudioSources();
        pointerWithinBounds = false;
    }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //pointerDownSound.Play();
        //pointerWithinBounds = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerDownSound.Play();
        pointerWithinBounds = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerEnterSound.Play();
        pointerWithinBounds = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointerWithinBounds)
        {
            pointerUpSound.Play();
        }
        pointerWithinBounds = false;
    }

    private void initializeAudioSources()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        pointerDownSound = audioSources[0];
        pointerEnterSound = audioSources[1];
    }

}
