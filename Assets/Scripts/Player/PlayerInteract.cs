using PickleMan;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInteract : MonoBehaviour, PlayerInputActions.IPlayerInteractActions
{
    public GameObject bobStartLocation;

    private PlayerInputActions playerInputActions;
    private Player player;
    private PlayerData playerData;
    private PlayerStates playerStates;
    private PlayerMovement playerMovement;
    private InteractPopup interactPopup;

    public List<GameObject> interactingObjects = new();

    private bool castHeld = false;
    private bool castReleased = false;
    private bool canCast = false;
    private bool canCatchFish = false;
    private bool catchInitiated = false;
    private int currentInteractObjectIndex = -1;

    private void Awake()
    {
        playerData = GameManager.Instance.playerData;
        player = GetComponent<Player>();
        playerStates = GetComponent<PlayerStates>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        playerInputActions = player.playerInputActions;
        playerInputActions.PlayerInteract.SetCallbacks(this);

        GameManager.Instance.gameData.OnFishCaught += CaughtFish;
    }

    private void OnDisable()
    {
        playerInputActions.PlayerInteract.RemoveCallbacks(this);

        GameManager.Instance.gameData.OnFishCaught -= CaughtFish;
    }

    private void Start()
    {
        interactPopup = UIGameManager.Instance.interactPopoup.GetComponent<InteractPopup>();
        playerData.currentFishingRod = GameManager.Instance.allFishingRodData[0];
    }

    private void Update()
    {
        UpdateFishingAction();
        UpdateInteractUI();
    }

    private void UpdateInteractUI()
    {
        if (interactingObjects.Count > 0)
        {
            currentInteractObjectIndex = GetBestInteractingObjectIndex();
            string interactingObjectStr = interactingObjects[currentInteractObjectIndex].GetComponent<InteractingObject>().GetDisplayString();
            interactPopup.ShowPopup(interactingObjectStr, GameManager.Instance.gameData.orangeTextColor);
        }
        else if (interactPopup.gameObject.activeSelf)
        {
            interactPopup.HidePopup();
        }
    }

    private int GetBestInteractingObjectIndex()
    {
        float smallestAngle = 999f;
        int currentIndex = -1;

        for (int i = 0; i < interactingObjects.Count; i++)
        {
            GameObject interactingObject = interactingObjects[i];
            Vector3 vectorToItem = interactingObject.transform.position - playerMovement.playerCamera.transform.position;
            float angleFromCameraToItem = Mathf.Abs(Vector3.Angle(vectorToItem, playerMovement.playerCamera.transform.forward));

            if (smallestAngle > angleFromCameraToItem)
            {
                smallestAngle = angleFromCameraToItem;
                currentIndex = i;
            }
        }

        return currentIndex;
    }

    private void UpdateFishingAction()
    {
        canCast = CanCast();
        canCatchFish = CanCatchFish();

        if (castHeld)
        {
            playerData.FishPowerCurrent += playerData.fishPowerIncreaseRate * Time.deltaTime;
        }
        else if (castReleased && canCast && playerData.FishPowerCurrent > 0)
        {
            CastRod();
        }
        else if (catchInitiated)
        {
            TryCatchFish();
        }
    }

    private void CastRod()
    {
        GameObject spawnedBob = Instantiate(GameManager.Instance.fishingRodBob);
        Rigidbody spawnedBobRb = spawnedBob.GetComponent<Rigidbody>();
        spawnedBobRb.position = bobStartLocation.transform.position;

        GameManager.Instance.DestroyCurrentBob();
        GameManager.Instance.SetCurrentBob(spawnedBob);

        Vector3 lookDirection = transform.GetComponent<PlayerMovement>().playerCamera.transform.forward;
        lookDirection = lookDirection.normalized;
        lookDirection = new Vector3(lookDirection.x, lookDirection.y, lookDirection.z);
        spawnedBobRb.velocity = lookDirection * playerData.FishPowerCurrent * playerData.currentFishingRod.fishingRodStats.maxCastDistance * 0.0075f;

        playerData.FishPowerCurrent = 0;
        castHeld = false;
        castReleased = true;
    }

    private void OnChargeRod()
    {
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.Charging;
        castHeld = true;
        castReleased = false;
    }

    private void OnReleaseRod()
    {
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.Casting;
        castHeld = false;
        castReleased = true;
    }

    private bool CanCast()
    {
        return GameManager.Instance.fishHooked ? false : true;
    }

    private bool CanCatchFish()
    {
        return true;
    }

    private void TryCatchFish()
    {
        // add in things here later

    }

    private void CaughtFish(FishData newFishData)
    {
        castHeld = false;
        castReleased = false;
        catchInitiated = false;
    }

    public void AddInteractingObject(GameObject newObject)
    {
        interactingObjects.Add(newObject);
    }

    public void RemoveInteractingObject(GameObject newObject)
    {
        interactingObjects.Remove(newObject);
    }

    public void OnFishPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            return;

        if (context.performed && !GameManager.Instance.fishHooked)
        {
            OnChargeRod();
        }
        else if (context.performed && GameManager.Instance.fishHooked)
        {
            castHeld = false;
            castReleased = false;
            catchInitiated = true;
        }
        else if (context.canceled)
        {
            OnReleaseRod();
        }
    }
     
    public void OnFishCancelled(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        castHeld = false;
        castReleased = false;
        catchInitiated = false;
        GameManager.Instance.fishHooked = false;
        GameManager.Instance.DestroyCurrentBob();
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.None;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || currentInteractObjectIndex == -1)
            return;

        GameObject currentInteractingObject = interactingObjects[currentInteractObjectIndex];
        if (currentInteractingObject.GetComponent<ItemSpawned>() != null)
        {
            ItemData itemData = currentInteractingObject.GetComponent<ItemSpawned>().itemData;
            int itemsRemaining = InventoryManager.Instance.inventory.TryAddItem(itemData);
            print($"Items Remaining {itemsRemaining}");
            if (itemsRemaining == 0)
            {
                Destroy(currentInteractingObject);
                interactingObjects.Remove(currentInteractingObject);
            }
        }
    }
}
