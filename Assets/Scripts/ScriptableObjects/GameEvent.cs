using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Zen Fisher/Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private Action listeners;

    public void RegisterListener(Action listener)
    {
        listeners += listener;
    }

    public void UnregisterListener(Action listener)
    {
        listeners -= listener;
    }

    public void Raise()
    {
        listeners?.Invoke();
    }
}