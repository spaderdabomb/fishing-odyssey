INCLUDE ../../Globals/globals.ink

EXTERNAL NPCDialogueFinish(npcID)

=== SetQuestState ===
{ 
- GettingStartedState == 0: -> GettingStarted_0
- GettingStartedState == 1: -> GettingStarted_1
}


=== GettingStarted_0 ===
What are you waiting for? Go catch a fish.
-> END

=== GettingStarted_1 ===
~NPCDialogueFinish("WiseOldMan")
Wow nice catch!!
-> END