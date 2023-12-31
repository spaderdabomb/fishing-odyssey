using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-91)]
public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance;

    public MiscEvents miscEvents;
    public QuestEvents questEvents;
    public InputEvents inputEvents;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        Instance = this;

        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        inputEvents = new InputEvents();
    }
}
