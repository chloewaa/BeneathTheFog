=== CollectCoinsStart ===
{ CollectCoinsQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START" : -> canStart
    - "IN_PROGRESS" : -> inProgress
    - "CAN_FINISH" : -> canFinish
    - "FINISHED" : -> finished
    - else: -> END
    }

= requirementsNotMet
-> END

= canStart
Hey!
You must be new around here.
My cat took my neighbor's coin pouch and now coins are scattered all over town!
Would you collect them for me and return them to Kaelen?
* [Sure.]
~StartQuest("CollectCoinsQuest")
Great! He lives right across the river from me.
-> DONE
* [No way.]
Leave me alone then.
-> END 

= inProgress
Have you collected the coins yet?
I bet Kaelen needs them!
-> END

= canFinish
You have the coins?
Go give them to Kaelen then.
-> END

= finished
Thanks for collecting the coins, you're a life saver!
-> END