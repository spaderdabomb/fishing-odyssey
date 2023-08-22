using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static InventoryManager Instance;

    [SerializeField] public VisualTreeAsset inventoryAsset;
    [SerializeField] public VisualTreeAsset inventorySlotAsset;
    [SerializeField] public int inventoryRows;
    [SerializeField] public int inventoryCols;

    public Inventory inventory { get; private set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        print("Inventory drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        print("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("End Drag");
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        VisualElement inventoryClone = inventoryAsset.CloneTree();
        inventory = new Inventory(inventoryClone, inventoryRows, inventoryCols);
        UIGameManager.Instance.uiGameScene.AddInventoryToPlayerInfo(inventoryClone);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
