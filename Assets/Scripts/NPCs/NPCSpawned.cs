using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Xml.Linq;

public class NPCSpawned : InteractingObject, IPersistable
{
    public NPCData npcDataAsset;
    [ReadOnly] public NPCData npcData;
    protected override string DisplayString { get; set; } = string.Empty;

    public const string storyIndexKey = "_storyIndex";
    public const string storyKey = "_story";
    string loadedStoryState;

    protected override void Start()
    {
        base.Start();
        npcData = Instantiate(npcDataAsset);
        DisplayString = "Talk to " + npcData.displayName;
        npcData.currentStory = new Story(npcDataAsset.npcStoryAsset.text);
        npcData.currentStory.state.LoadJson(loadedStoryState);
    }

    public void SaveData()
    {
        ES3.Save(npcData.npcID + storyKey, npcData.currentStory.state.ToJson());
    }

    public void LoadData()
    {
        print("Loading data");
        //npcData.currentStory = new Story(npcDataAsset.npcStoryAsset.text);
        loadedStoryState = ES3.Load<string>(npcDataAsset.npcID + storyKey, defaultValue: new Story(npcDataAsset.npcStoryAsset.text).state.ToJson());
       // npcData.currentStory
    }
}
