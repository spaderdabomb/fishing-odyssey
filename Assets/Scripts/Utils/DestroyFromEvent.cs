using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyFromEvent : MonoBehaviour
{
    [SerializeField] GameEvent onDestroyEvent;

    private void OnEnable()
    {
        onDestroyEvent.RegisterListener(Destroy);
    }

    private void OnDisable()
    {
        onDestroyEvent.UnregisterListener(Destroy);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
