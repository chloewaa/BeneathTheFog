== collectCoinsFinish ==
{ CollectCoinsQuestState: 
    - "FINISHED": -> finished
    - else: -> default
}
    
= finished
thank you!
-> END

= default 
What do you want?
*[Nothing, never mind]
-> END
* {CollectCoinsQuestState == " CAN_FINISH"} [Here are the coins you lost]
 ~FinishQuest(CollectCoinsQuestId)
 Oh wow! Thanks!
 -> END