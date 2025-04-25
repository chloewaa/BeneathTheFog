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
Huh...a stranger.
Have you seen the fog closing in around the village?
This fog takes people, it twists their minds.
They become shells of their former selves.
Poor Kaelen's daughter wandered too far into the fog yesterday...
* [Can I help?]
~StartQuest("CollectCoinsQuest")
If you really want to, you can collect some Mistleaf.
It's usually scattered around the village from the wind. 
It may be enough to wake her from her sleep. 
-> DONE
* [Well, that's too bad.]
It is indeed.
Perhaps she'll never wake...
-> END 

= inProgress
Have you collected the leaves yet?
I bet Kaelen's daughter could use them.
-> END

= canFinish
You have the leaves?
Go give them to Kaelen then.
-> END

= finished
Thanks for collecting the leaves, lets hope she wakes up. 
-> END