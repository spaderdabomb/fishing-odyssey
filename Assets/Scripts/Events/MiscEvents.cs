using System;

public class MiscEvents
{
    public event Action<FishData> onFishCaught;
    public void FishCaught(FishData fishData)
    {
        if (onFishCaught != null)
        {
            onFishCaught(fishData);
            if (!DataManager.Instance.FishCaughtDict.ContainsKey(fishData.fishID))
            {
                NewFishCaught(fishData);
            }
        }
    }

    public event Action<FishData> onNewFishCaught;
    public void NewFishCaught(FishData fishData)
    {
        if (onNewFishCaught != null)
        {
            onNewFishCaught(fishData);
        }
    }

    public event Action onGemCollected;
    public void GemCollected()
    {
        if (onGemCollected != null)
        {
            onGemCollected();
        }
    }


    public event Action<string> onNPCDialogueFinish;
    public void NPCDialogueFinish(string npcID)
    {
        if (onNPCDialogueFinish != null)
        {
            onNPCDialogueFinish(npcID);
        }
    }
}