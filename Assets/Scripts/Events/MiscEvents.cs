using System;

public class MiscEvents
{
    public event Action onGemCollected;
    public void GemCollected() => onGemCollected?.Invoke();

    public event Action<string> onNPCDialogueFinish;
    public void NPCDialogueFinish(string npcID) => onNPCDialogueFinish?.Invoke(npcID);

}