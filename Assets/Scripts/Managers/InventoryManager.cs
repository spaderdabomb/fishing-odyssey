using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using PickleMan;
using ZenUI;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class InventoryManager : SerializedMonoBehaviour
{
    public static InventoryManager Instance;

    public VisualTreeAsset inventoryAsset;
    public VisualTreeAsset inventorySlotAsset;
    public VisualTreeAsset gearContainerAsset;
    public VisualTreeAsset gearSlotAsset;

    public int inventoryRows;
    public int inventoryCols;

    private Vector2 startMousePosition;
    private Vector2 startElementPosition;
    public float timeToShowTooltip = 0.5f;
    [HideInInspector] public InventorySlot CurrentHoverSlot { get; set; } = null;
    [HideInInspector] public bool IsHoveringSlot { get; set; } = false;
    [HideInInspector] public float TimeSinceHoveringSlot { get; set; } = 0f;


    private Player player;
    public Inventory inventory { get; private set; } = null;
    public PopupMenuInventory popupMenuInventory { get; private set; } = null;

    public Dictionary<GearContainerType, GearContainer> gearContainerDict = new();
    public Dictionary<GearContainerType, Dictionary<GearContainerSlotTypes, GearSlotData>> gearContainerToDataDicts;

    public Dictionary<GearContainerSlotTypes, GearSlotData> handsSlotsDataDict = new();
    public Dictionary<GearContainerSlotTypes, GearSlotData> tackleSlotsDataDict = new();
    public Dictionary<GearContainerSlotTypes, GearSlotData> outfitSlotsDataDict = new();
    public Dictionary<GearContainerSlotTypes, GearSlotData> accessoriesSlotsDataDict = new();

    private void Awake()
    {
        Instance = this;
        player = GameManager.Instance.player.GetComponent<Player>();

        gearContainerToDataDicts = new()
        {
            { GearContainerType.Hands, handsSlotsDataDict },
            { GearContainerType.Tackle, tackleSlotsDataDict },
            { GearContainerType.Outfit, outfitSlotsDataDict },
            { GearContainerType.Accessories, accessoriesSlotsDataDict }
        };
    }

    void Start()
    {
        VisualElement inventoryClone = inventoryAsset.CloneTree();
        inventory = new Inventory(inventoryClone, inventoryRows, inventoryCols);
        popupMenuInventory = InitPopupMenu();
        UIGameManager.Instance.uiGameScene.AddInventoryToPlayerInfo(inventoryClone);

        foreach (GearContainerType gearContainerType in Enum.GetValues(typeof(GearContainerType)))
        {
            VisualElement gearContainerClone = gearContainerAsset.CloneTree();
            gearContainerDict.Add(gearContainerType, new GearContainer(gearContainerClone, gearContainerType));
            UIGameManager.Instance.uiGameScene.AddElementToGearContainer(gearContainerClone);
        }
    }

    private PopupMenuInventory InitPopupMenu()
    {
        VisualElement popupMenuAsset = UIGameManager.Instance.popupMenuInventory.CloneTree();
        PopupMenuInventory newPopupMenuInventory = new PopupMenuInventory(popupMenuAsset);
        UIGameManager.Instance.uiGameScene.root.Add(popupMenuAsset);
        newPopupMenuInventory.root.style.position = Position.Absolute;
        newPopupMenuInventory.root.style.display = DisplayStyle.Flex;

        return newPopupMenuInventory;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHoveringTime();
    }

    private void CheckHoveringTime()
    {
        if (IsHoveringSlot)
        {
            if (TimeSinceHoveringSlot > timeToShowTooltip)
                ShowInventoryTooltip();
            else
                HideInventoryTooltip();

            TimeSinceHoveringSlot += Time.deltaTime;
        }
        else
        {
            TimeSinceHoveringSlot = 0f;
            HideInventoryTooltip();
        }
    }

    public void ShowInventoryTooltip()
    {
        if (inventory.currentHoverSlot?.currentItemData == null)
            return;

        Vector2 positionDiff = inventory.currentHoverSlot.root.ChangeCoordinatesTo(popupMenuInventory.root.parent, inventory.currentHoverSlot.root.layout.position);
        popupMenuInventory.root.style.display = DisplayStyle.Flex;
        popupMenuInventory.root.style.left = inventory.currentHoverSlot.root.resolvedStyle.left + positionDiff.x + 45f;
        popupMenuInventory.root.style.top = inventory.currentHoverSlot.root.resolvedStyle.top + positionDiff.y + 85f;

        if (!popupMenuInventory.itemDataShowing)
        {
            popupMenuInventory.SetItemData(inventory.currentHoverSlot.currentItemData);
        }
    }

    public void HideInventoryTooltip()
    {
        popupMenuInventory.root.style.display = DisplayStyle.None;
        popupMenuInventory.RemoveItemData();
    }


    public ItemData InstantiateItem(ItemData itemData)
    {
        ItemData newItemData = Instantiate(itemData.itemDataAsset);

        return newItemData;
    }

    public void InstantiateItemSpawned(ItemData itemData)
    {
        GameObject newItemSpawned = Instantiate(itemData.item3DPrefab, GameManager.Instance.itemContainer.transform);
        ItemSpawned newItemSpanwedInst = newItemSpawned.GetComponent<ItemSpawned>();
        newItemSpanwedInst.InitItemData();
        newItemSpanwedInst.itemData.stackCount = itemData.stackCount;

        Transform playerTransform = player.transform;
        Camera playerCamera = GameManager.Instance.playerMovement.playerCamera;

        newItemSpawned.transform.position = playerCamera.transform.position + playerTransform.forward * 2;
        Rigidbody newItemRb = newItemSpawned.GetComponent<Rigidbody>();
        newItemRb.velocity = new Vector3(Random.Range(0f, 1f), Random.Range(0.5f, 2f), Random.Range(0f, 1f));
        newItemRb.angularVelocity = new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f));
    }

    public void BeginDragHandler(PointerDownEvent evt, InventorySlot inventorySlot)
    {
        // No item exists
        if (inventorySlot.currentItemData == null || evt.button != 0)
            return;

        inventory.currentDraggedInventorySlot = inventorySlot;
        inventory.currentDraggedInventorySlot.SetTinted();

        ShowGhostIcon(evt, inventorySlot.currentItemData.itemSprite.texture, inventorySlot.currentItemData.stackCount);

        evt.StopPropagation();
    }

    public void MoveDragHandler(PointerMoveEvent evt)
    {
        VisualElement ghostIcon = inventory.GetGhostIconRef();
        if (ghostIcon.HasPointerCapture(evt.pointerId))
        {
            Vector2 displacement = new Vector2(evt.position.x, evt.position.y) - startMousePosition;
            ghostIcon.style.left = startElementPosition.x + displacement.x;
            ghostIcon.style.top = startElementPosition.y + displacement.y;

            InventorySlot currentInventoryHoverSlot = inventory.GetCurrentSlotMouseOver(evt);
            inventory.SetCurrentSlot(currentInventoryHoverSlot);
            GearSlot currentGearHoverSlot = UpdateCurrentGearHoverSlot(evt);
        }
    }

    public void EndDragHandler(PointerUpEvent evt)
    {
        VisualElement ghostIcon = inventory.GetGhostIconRef();
        if (ghostIcon.HasPointerCapture(evt.pointerId))
        {
            InventorySlot closestSlot = inventory.GetClosestSlotToRelease();
            if (closestSlot != null)
            {
                inventory.MoveItem(closestSlot.slotIndex, inventory.currentDraggedInventorySlot.slotIndex);
            }

            ghostIcon.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
            ghostIcon.style.left = 0f;
            ghostIcon.style.top = 0f;
            ghostIcon.style.visibility = Visibility.Hidden;

            inventory.currentDraggedInventorySlot.SetTintNormal();
            inventory.currentDraggedInventorySlot = null;
        }
    }

    private void ShowGhostIcon(PointerDownEvent evt, Texture2D bgTexture, int stackCount)
    {
        VisualElement ghostIcon = inventory.GetGhostIconRef();
        Label ghostIconLabel = inventory.GetGhostIconLabelRef();
        ghostIcon.style.position = Position.Absolute;
        ghostIcon.style.visibility = Visibility.Visible;
        ghostIcon.style.backgroundImage = bgTexture;
        ghostIconLabel.text = stackCount.ToString();

        startMousePosition = evt.position;
        float positionLeft = ghostIcon.WorldToLocal(evt.position).x - ghostIcon.resolvedStyle.width / 2;
        float positionTop = ghostIcon.WorldToLocal(evt.position).y - ghostIcon.resolvedStyle.height / 2;
        ghostIcon.style.left = positionLeft;
        ghostIcon.style.top = positionTop;
        startElementPosition = new Vector2(positionLeft, positionTop);

        ghostIcon.CapturePointer(evt.pointerId);
    }

    public GearSlot UpdateCurrentGearHoverSlot(PointerMoveEvent evt)
    {
        GearSlot currentGearSlot = null;
        foreach (GearContainer currentGearContainer in gearContainerDict.Values)
        {
            GearSlot tempGearSlot = currentGearContainer.GetCurrentSlotMouseOver(evt);
            currentGearSlot = tempGearSlot != null ? currentGearSlot : null;
            currentGearContainer.SetCurrentSlot(currentGearSlot);
        }

        return currentGearSlot;
    }

    public void UpdateCurrentHoverSlot(InventorySlot newHoverSlot, bool isHovering)
    {
        if (isHovering)
        {
            CurrentHoverSlot = newHoverSlot;
            IsHoveringSlot = true;
        }
        else
        {
            CurrentHoverSlot = null;
            IsHoveringSlot = false;
            TimeSinceHoveringSlot = 0f;
        }
    }
}
