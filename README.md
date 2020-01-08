# Life_Systems
Life System developed on Unity 2017 using C# by Javier Dalma.

This System works like the one on Candy Crush it gives you a certain amount of lifes and you get a life after a certain amount of time.

LMConfig.cs: this script contains all the constants that the system needs, like amount of life slots, amount of time to refill etc.

LivesManager.cs: here we get all the values that are stored on the LMConfig.cs. This script handles the amount o lifes the player has, the timer to get another life, if the player gains a life, get another life slot (if possible on the game) also having infinite times for a certain amount of time. This code also handles the timer to get a new life and has the variables to assign the UI texts.

LifesControl.cs: and finally this Script handles the function to give the player 1 life or a certain amount of lifes, loose a life, give unlimitad lifes for a certain amount of time, get an extra life slot. And allows the player to play if he has at least 1 life.

The code has coments to understand every function
