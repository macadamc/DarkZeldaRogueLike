using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
[RequireComponent (typeof(AudioSource))]
public class BaseInteractable : MonoBehaviour, iInteractable
{
    public enum InteractType { ON_ENTER, ON_EXIT, ON_INTERACT }

    public InteractType interactType;

    public AudioClip interactSFX;
    [HideInInspector]
    public AudioSource source;

    public Collider2D trigger;

    public void PlaySFX(AudioClip sfx)
    {
        source.Stop();
        source.clip = sfx;
        source.Play();
    }

    public virtual void Awake()
    {
        trigger = GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false && other.CompareTag("Entity") == false) { return; }

        if (interactType == InteractType.ON_ENTER)
        {
            Interact(other.GetComponent<Entity>());
        }
        else if (interactType == InteractType.ON_INTERACT)
        {
            Entity e = other.GetComponent<Entity>();
            e.targetInteractable = this;
        }

    }
    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false && other.CompareTag("Entity") == false) { return; }

        if (interactType == InteractType.ON_EXIT)
        {
            Interact(other.GetComponent<Entity>());
        }
        else if (interactType == InteractType.ON_INTERACT)
        {
            Entity e = other.GetComponent<Entity>();
            if (e.targetInteractable == this) { e.targetInteractable = null; }
        }
    }

    public virtual void Interact(Entity other)
    {
        throw new NotImplementedException();
    }
}