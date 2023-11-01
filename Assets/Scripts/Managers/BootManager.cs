using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class BootManager : MonoBehaviour
{
    public static BootManager Instance;

    [SerializeField] private GameObject dataManager;
    [SerializeField] private GameObject audioManager;
    [SerializeField] private GameObject inputManager;
    [SerializeField] private GameObject debugManager;

    private GameObject spawnedDataManager;
    private GameObject spawnedInputManager;
    private GameObject spawnedDebugManager;
    private GameObject spawnedaudioManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InstantiateManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InstantiateManagers()
    {
        spawnedDataManager = Instantiate(dataManager);
        spawnedaudioManager = Instantiate(audioManager);
        spawnedInputManager = Instantiate(inputManager);
        spawnedDebugManager = Instantiate(debugManager);
    }
}
