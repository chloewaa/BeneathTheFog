EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)

//quest names
VAR CollectCoinsQuestId = "CollectCoinsQuest"

//quest states (quest id + "State")
VAR CollectCoinsQuestState = "REQUIREMENTS_NOT_MET"

INCLUDE CollectCoinsStartNPC.ink
INCLUDE CollectCoinsEndNPC.ink