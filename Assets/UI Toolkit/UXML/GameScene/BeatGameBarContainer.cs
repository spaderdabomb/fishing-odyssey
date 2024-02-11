using UnityEngine.UIElements;
using UnityEngine;
using Unity.AppUI.UI;
using System.Collections.Generic;
using static UnityEngine.Rendering.HableCurve;

public partial class BeatGameBarContainer
{
    public int NumSegments { get; private set; } = 0;

    private int currentSegment = 0;
    private VisualElement root;
    private List<VisualElement> segments = new List<VisualElement>();
    public BeatGameBarContainer(VisualElement root)
    {
        AssignQueryResults(root);
        RegisterCallbacks();

        this.root = root;
    }

    private void RegisterCallbacks()
    {
        GameEventsManager.Instance.fishingEvents.onBeatNoteSubmitted += IncreaseSegmentsComplete;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing += HideBeatGameBar;
    }

    private void UnregisterCallbacks()
    {
        GameEventsManager.Instance.fishingEvents.onBeatNoteSubmitted -= IncreaseSegmentsComplete;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing -= HideBeatGameBar;
    }

    public void ShowBeatGameBar(int numSegments, Vector3 screenPosition)
    {
        UpdatePosition(screenPosition);
        NumSegments = numSegments;
        root.style.display = DisplayStyle.Flex;

        for (int i = 0; i < NumSegments; i++)
        {
            VisualElement newSegment = new VisualElement();

            if (i == 0)
            {
                newSegment.AddToClassList("beat-bar-segment-left");
            }
            else if (i == NumSegments - 1)
            {
                newSegment.AddToClassList("beat-bar-segment-right");
            }
            else
            {
                newSegment.AddToClassList("beat-bar-segment-middle");
            }

            newSegment.styleSheets.Add(UIGameManager.Instance.beatBarStylesheet);
            newSegment.style.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
            beatGameBarOutline.Add(newSegment);
            segments.Add(newSegment);
        }
    }

    public void IncreaseSegmentsComplete(BeatNoteRegion beatNoteRegion)
    {
        int newSegments = BeatSequencer.Instance.beatNoteRegionsToSegmentsDict[beatNoteRegion];
        for (int i = 0; i < newSegments; i++)
        {
            if (currentSegment >= NumSegments)
                break;

            VisualElement currentSegmentElement = segments[currentSegment];
            currentSegmentElement.style.backgroundColor = GameManager.Instance.gameData.standardBrightYellow;
            currentSegment++;
        }
    }

    public void HideBeatGameBar()
    {
        root.style.display = DisplayStyle.None;
        currentSegment = 0;
        beatGameBarOutline.Clear();
        segments.Clear();
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        this.root.style.left = newPosition.x;
        this.root.style.top = newPosition.y;
    }
}
