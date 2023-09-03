using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace ZenUI
{
    public partial class GearContainer
    {
        VisualElement root;
        public GearContainerType gearContainerType;
        public List<GearSlot> gearSlots = new();
        public List<GearSlotData> gearSlotData = new();
        public GearSlot currentHoverSlot = null;
        public Dictionary<GearContainerSlotTypes, GearSlotData> gearSlotDataDict = new();

        private int numRows = 1;
        private int numColumns = 6;
        public GearContainer(VisualElement root, GearContainerType gearContainerType)
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
            foreach (var keyValuePair in gearSlotDataDict.Values)
            {
                gearSlotData.Add(keyValuePair);
            }
        }

        public void InitGearSlots()
        {
            for (int i = 0; i < numColumns; i++) 
            {
                VisualElement gearSlotClone = InventoryManager.Instance.gearSlotAsset.CloneTree();
                GearSlotData tempGearSlotData = i >= gearSlotData.Count ? null : gearSlotData[i];
                GearSlot newGearSlot = new GearSlot(gearSlotClone, i, this, tempGearSlotData);

                newGearSlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, newGearSlot));

                gearSlots.Add(newGearSlot);
                gearSlotContainer.Add(gearSlotClone);

            }
        }

        // WARNING: Only works with pointer/mouse events, does not work with general Input.mousePosition
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
    }

}
