using System;

public class MiscEvents
{
    public event Action onFishCollected;
    public void FishCollected()
    {
        if (onFishCollected != null)
        {
            onFishCollected();
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