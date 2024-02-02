using Dialogue;
using JSAM;
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
    private PlayerAnimation playerAnimation;
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
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void OnEnable()
    {
        playerInputActions = player.playerInputActions;
        playerInputActions.PlayerInteract.SetCallbacks(this);

        GameEventsManager.Instance.fishingEvents.onFishCaught += CaughtFish;
    }

    private void OnDisable()
    {
        playerInputActions.PlayerInteract.RemoveCallbacks(this);
        GameEventsManager.Instance.fishingEvents.onFishCaught -= CaughtFish;
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
        FishingRodBob newBob = spawnedBob.GetComponent<FishingRodBob>();
        newBob.OnInstantiate(playerData);

        GameEventsManager.Instance.fishingEvents.CastRod(spawnedBob);

        playerData.FishPowerCurrent = 0;
        castHeld = false;
        castReleased = true;
    }

    private void OnChargeRod()
    {
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.Charging;
        playerAnimation.PlayerFishingAnimationState = PlayerFishingAnimationState.Charging;

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
        return GameManager.Instance.FishIsHooked ? false : true;
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
        if (context.started || GameManager.Instance.FishIsSpawned)
            return;

        if (context.performed && !GameManager.Instance.FishIsHooked)
        {
            OnChargeRod();
        }
        else if (context.performed && GameManager.Instance.FishIsHooked)
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

        GameEventsManager.Instance.fishingEvents.StoppedFishing();
    }

    public void OnHookFish(InputAction.CallbackContext context)
    {
        if (!context.performed || !GameManager.Instance.FishIsSpawned || GameManager.Instance.FishIsHooked)
            return;

        GameEventsManager.Instance.fishingEvents.FishHooked(GameManager.Instance.CurrentFishData);
    }

    public void OnBeatNotePressed(InputAction.CallbackContext context)
    {
        if (!context.performed || !GameManager.Instance.FishIsHooked)
            return;

        BeatSequencer.Instance.BeatNotePressed();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || currentInteractObjectIndex == -1 || interactingObjects.Count <= 0)
            return;

        // GameEventsManager.Instance.inputEvents.SubmitPressed();


        GameObject currentInteractingObject = interactingObjects[currentInteractObjectIndex];
        if (currentInteractingObject.GetComponent<ItemSpawned>() != null)
        {
            InteractWithItem(currentInteractingObject);
        }
        else if (currentInteractingObject.GetComponent<NPCSpawned>() != null)
        {
            InteractWithNPC(currentInteractingObject);
        }
    }

    public void InteractWithItem(GameObject interactingObject)
    {
        ItemData itemData = interactingObject.GetComponent<ItemSpawned>().itemData;
        int itemsRemaining = InventoryManager.Instance.inventory.TryAddItem(itemData);

        if (itemsRemaining < itemData.stackCount)
        {
            AudioManager.PlaySound(MainAudioLibrarySounds.ItemPickup);
        }

        if (itemsRemaining == 0)
        {
            Destroy(interactingObject);
            interactingObjects.Remove(interactingObject);
        }
    }

    public void InteractWithNPC(GameObject interactingObject)
    {
        UIGameManager.Instance.SetPlayerInMenuOptions(MenuType.Dialogue);
        DialogueManager.Instance.EnterDialogueMode(interactingObject.GetComponent<NPCSpawned>().npcData.currentStory);
    }
}
