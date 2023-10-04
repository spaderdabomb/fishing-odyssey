INCLUDE ../../Quests/GettingStarted.ink

EXTERNAL StartQuest()

{ 
- GettingStartedState == -2: -> WiseOldMan_NiceDay
- GettingStartedState == -1: -> WiseOldMan_Main
- GettingStartedState >= 0: -> SetQuestState
}

=== WiseOldMan_Main ===
Welcome to Zen Fisher!
I found you out by the lake over in the Grassy Biome, where did you come from?
Anyways, if you're gonna live here, you're gonna have to catch your share of fish.
~StartQuest()
I have a spare rod in the barrel over there, why don't you try and have a go at it?
-> END

=== WiseOldMan_NiceDay ===
Nice day today innit?
-> END