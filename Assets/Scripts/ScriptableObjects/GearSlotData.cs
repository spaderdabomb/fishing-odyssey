using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenUI;
using Sirenix.OdinInspector.Editor;
using System;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "GearSlotData", menuName = "Fishing Odyssey/Gear/GearSlotData")]
public class GearSlotData : SerializedScriptableObject
{
    public string displayName;
    public GearContainerType gearContainerType;
    public Texture2D backingIcon;
}

public enum GearContainerType
{
    Hands,
    Tackle,
    Outfit,
    Accessories
}

public enum GearContainerSlotTypes
{
    // Tackle
    Hook,
    Bait,
    Line,
    Reel,
    Lure,
    Special,
    // Outfit
    Head,
    Body,
    Cape,
    Legs,
    Gloves,
    Boots,
    // Acccessories
    Amulet,
    Ring,
    Socks,
    Badge,
    Tattoo,
    Earrings
}