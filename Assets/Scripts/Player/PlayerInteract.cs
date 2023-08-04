using PickleMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour, PlayerInput.IPlayerInteractActions
{
    public GameObject bobStartLocation;

    private PlayerInput playerInput;
    private PlayerData playerData;
    private PlayerStates playerStates;

    private bool castHeld = false;
    private bool castReleased = false;
    private bool canCast = false;
    private bool canCatchFish = false;
    private bool catchInitiated = false;

    private void Awake()
    {
        playerData = GameManager.Instance.playerData;
        playerStates = GetComponent<PlayerStates>();
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.PlayerInteract.SetCallbacks(this);

        GameManager.Instance.gameData.OnFishCaught += CaughtFish;
    }

    private void OnDisable()
    {
        playerInput.PlayerInteract.RemoveCallbacks(this);

        GameManager.Instance.gameData.OnFishCaught -= CaughtFish;
    }

    private void Start()
    {
        playerData.currentFishingRod = GameManager.Instance.allFishingRodData[0];
    }

    private void Update()
    {
        UpdateFishingAction();
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
}
