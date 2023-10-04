using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using static ItemData;

namespace ZenUI
{
    public partial class GearContainer : BaseSlotContainer
    {
        public GearContainerType gearContainerType;
        public List<GearSlot> gearSlots = new();
        public List<GearSlotData> gearSlotData = new();
        public List<ItemType> gearSlotKeys = new();
        public Dictionary<ItemType, GearSlotData> gearSlotDataDict = new();

        public GearContainer(VisualElement root, int inventoryRows, int inventoryCols, GearContainerType gearContainerType) : base(root, inventoryRows, inventoryCols)
        {
            this.root = root;
            this.gearContainerType = gearContainerType;
            this.root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            gearSlotDataDict = InventoryManager.Instance.gearContainerToDataDicts[gearContainerType];

            AssignQueryResults(root);
            InitGearContainer();
            InitGearSlots();
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void InitGearContainer()
        {
            gearSubHeaderLabel.text = gearContainerType.ToString().ToUpper();
            foreach (var keyValuePair in gearSlotDataDict)
            {
                gearSlotData.Add(keyValuePair.Value);
                gearSlotKeys.Add(keyValuePair.Key);
            }
        }

        public void InitGearSlots()
        {
            // Init slots
            inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < inventoryCols; i++) 
            {
                VisualElement gearSlotClone = InventoryManager.Instance.gearSlotAsset.CloneTree();
                GearSlotData tempGearSlotData = i >= gearSlotData.Count ? null : gearSlotData[i];
                ItemType tempItemType = i >= gearSlotKeys.Count ? ItemType.None : gearSlotKeys[i];
                GearSlot newGearSlot = new GearSlot(gearSlotClone, i, this, tempGearSlotData, tempItemType);
                newGearSlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, newGearSlot));
                gearSlots.Add(newGearSlot);
                inventorySlots.Add(newGearSlot);
                gearSlotContainer.Add(gearSlotClone);
            }

            // Init persistent data
            BaseItemData[] baseItemData = DataManager.Instance.LoadGearContainerData(gearContainerType);
            for (int i = 0; i < baseItemData.Length; i++)
            {
                BaseItemData baseItem = baseItemData[i];
                if (baseItem != null)
                {
                    ItemData itemDataAsset = ItemExtensions.GetItemData(baseItem.itemID);
                    ItemData newItem = itemDataAsset.GetItemDataInstantiated();
                    newItem.SetItemDataToBaseItemData(baseItem);
                    AddItem(newItem, inventorySlots[i]);
                }
            }
        }

        public GearSlot GetCurrentSlotMouseOver(PointerMoveEvent evt)
        {
            GearSlot currentSlot = null;
            foreach (GearSlot slot in gearSlots)
            {
                if (slot.root.worldBound.Contains(evt.position))
                {
                    currentSlot = slot;
                    break;
                }
            }

            return currentSlot;
        }

        public void SetCurrentSlot(GearSlot newGearSlot)
        {
            currentHoverSlot = newGearSlot;
        }

        public override bool CanMoveItem(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
        {
            if (!base.CanMoveItem(dragEndSlot, dragBeginSlot))
            {
                return false;
            }

            GearSlot dragEndGearSlot = (GearSlot)dragEndSlot;

            bool validItemType = dragEndGearSlot.itemType == dragBeginSlot.currentItemData.itemType;
            bool validHandsType = InventoryManager.Instance.IsValidHandsSlotItem(dragBeginSlot.currentItemData);
            bool isHandsContainer = dragEndGearSlot.gearContainer.gearContainerType == GearContainerType.Hands;

            bool canMoveItem = validItemType || (validHandsType && isHandsContainer);

            return canMoveItem;
        }
    }

}
