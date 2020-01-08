using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifesControl : MonoBehaviour {

    // Variable that will hold an instance of LivesManager
    LivesManager lm = null;

    // Use this for initialization
    void Start()
    {

        // Get LivesManager object from the scene
        GameObject gameObject = GameObject.Find("LivesManager");

        // If LivesManager object exist
        if (gameObject != null)
        {

            // Get LivesManager component
            lm = gameObject.GetComponent<LivesManager>();
        }

    }

    //get one life
    public void refillOneLive()
    {
        if (lm)
        {
            if (lm.canRefillLives())
            {
                lm.refillOneLife();
            }
        }
    }



    //Lose one life
    public void looseOneLife()
    {
        if (lm)
        {
            if (lm.canLooseLife())
            {
                lm.looseOneLife();
            }
        }
    }

    // Refill all lives
    public void refillAllLives()
    {
        if (lm)
        {
            if (lm.canRefillLives())
            {
                lm.refillAllLives();
            }
        }
    }

    //Get unlimited lives
    public void getUnlimitedLives()
    {
        if (lm)
        {
            if (lm.canGetUnlimitedLives())
            {
                lm.getUnlimitedLives();
            }
        }
    }

    //Get an extra life slot
    public void getExtraLifeSlot()
    {

        if (lm)
        {
            if (lm.canGetExtraLifeSlot())
            {
                lm.getExtraLifeSlot();
            }

        }
    }

    public void canPlay()
    {
        if (lm)
        {
            if (lm.canPlay())
                GameObject.Find("TEXT_DEBUG").GetComponent<Text>().text = "Debug: You have enough lives to play!";
            else
                GameObject.Find("TEXT_DEBUG").GetComponent<Text>().text = "Debug: You are out of lives and cannot play!";
        }
    }



    //Use for testing
    public void reset()
    {
        if (lm)
            lm.reset();
    }
}
