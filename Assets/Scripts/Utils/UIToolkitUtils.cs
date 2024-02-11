using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIToolkitUtils
{
    public static Vector3 WorldSpaceToScreenSpace(Vector3 position)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        screenPosition.y = Screen.height - screenPosition.y;

        return screenPosition;
    }
}
