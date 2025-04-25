== collectCoinsFinish ==
{ CollectCoinsQuestState: 
    - "FINISHED": -> finished
    - else: -> default
}
    
= finished
thank you for your help.
-> END

= default 
What do you want?
*[Nothing, never mind]
-> END
* {CollectCoinsQuestState == "CAN_FINISH"} [I brought some Mistleaf.]
 ~FinishQuest(CollectCoinsQuestId)
 Mistleaf? 
 I suppose the scent could wake her.
 Thank you for collecting this for my daughter.
 -> END