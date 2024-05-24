EXTERNAL TestDebug(str)
VAR introductions = 0
{introductions==0:-> intro | -> bye}

== intro ==
Hello! #speaker:Joyce
~ TestDebug("Joyce says Hi")
Welcome to My Character :D 
* [Who are you?]
    I'm <color="blue"><b>Joyce</b></color>!
    ~ introductions = 1
* [Yay!]
    Yay!

-Have a good day! #speaker:Player
-> bye

== bye ==
Goodbye! #speaker:Joyce
-> END