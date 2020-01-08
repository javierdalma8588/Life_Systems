using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
public class LivesManager : MonoBehaviour {
    
	#region CONSTANTS

	// Ids to be used for persistent storage
	const string ID_CURRENT_LIVES = "lm_current_lives";
	const string ID_REGENERATION_TIMESTAMP = "lm_reset_timestamp";
	const string ID_UNLIMITED_TIMESTAMP = "lm_unlimited_timestamp";
	const string ID_EXTRA_LIVE_SLOTS = "lm_extra_slots";
	const string ID_FIRST_TIME = "lm_first_time";

	#endregion

	#region VARIABLES

	// The current lives the player has
	public int currentLives;

	// Any extra live slots the user has purchased 
	int extraLives;

	// The timestamp a life should refill (in seconds)
	string regenerationTimestamp;

	// The timestamp unlimited lives should end (in seconds)
	string unlimitedTimestamp;

	#endregion

	#region EXTERNAL_API

	// Return true if you have more than 0 lives or unlimited lives.
	public bool canLooseLife(){
		return currentLives > 0 && !isUnlimitedLives();
	}

	// Remove 1 life from current lives
	public void looseOneLife(){ 

		// If you already have 0 lives, do nothing
		if (currentLives == 0)
			return;

		// Update current lives variable
		currentLives--;

		// Sync current lives variable with persistent storage
		PlayerPrefs.SetInt (ID_CURRENT_LIVES, currentLives);
		PlayerPrefs.Save ();

		// If you had full lives, start the refill timer
		if (currentLives == getMaxNumberOfLives ()-1) {
			setLifeRegenerationTimer ();
			InvokeRepeating ("checkRegenerationTime", 0.0f, 1.0f);
		}
	}

	/*
	 * Returns true if you dont have unlimited lives 
	 * and you can refill atleast one life
	 */
	public bool canRefillLives(){
		return currentLives <getMaxNumberOfLives() && !isUnlimitedLives();
	}

	// Add 1 life
	public void refillOneLife(){

        //Debug.LogError("dame 1 vida");
		// If you already have full lives, do nothing
		if (currentLives == getMaxNumberOfLives ())
			return;
            currentLives++;
			PlayerPrefs.SetInt (ID_CURRENT_LIVES, currentLives);
			PlayerPrefs.Save ();
			// Reset the refill timer
        if (currentLives<getMaxNumberOfLives() && GameControll.instance._guiManager.mainScreenCanvas.activeSelf == true)
				setLifeRegenerationTimer ();

        GameControll.instance._guiManager.currentLivesText.text = currentLives.ToString();
         
	}

	// Refill all lives
	public void refillAllLives(){
		while (currentLives < getMaxNumberOfLives()) 
			refillOneLife ();
	}

	// Returns true if you can buy unlimited lives in-app (you don't already have unlimited lives)
	public bool canGetUnlimitedLives(){
		return !isUnlimitedLives ();
	}

	// Refill specified number lives
	public void refillXLives(int livesToAdd){
        for (int i=0; i<livesToAdd; i++)
			refillOneLife();
        //looseOneLife();
	}
		
	// Get Unlimited lives
	public void getUnlimitedLives(){

		// If you already have unlimited lives, do nothing
		if (isUnlimitedLives ())
			return;

		refillAllLives ();
		setUnlimitedTimer ();
		InvokeRepeating ("checkUnlimitedTime", 0.0f, 1.0f);
	}

	/* 
	 * Check if you can purchase an extra life slot. 
	 * Ideal in case you want to enable/disable the "purchase extra life slot" button.
	 */
	public bool canGetExtraLifeSlot(){ 
		if (extraLives < LMConfig.MAX_EXTRA_LIFE_SLOTS)
			return true;

		return false;
	}

	// Purchase an extra life slot
	public void getExtraLifeSlot(){
		extraLives++;
		PlayerPrefs.SetInt (ID_EXTRA_LIVE_SLOTS,extraLives);
		PlayerPrefs.Save ();
		refillAllLives ();
		updateUserInterface ();
	}
		
	/* 
	 * Check if there are lives left to play
	 * Ideal to use in order to decide if you will take the user to gamescene,
	 * or show him the "out of lives popup"
	 */
	public bool canPlay(){
		return isUnlimitedLives () || currentLives > 0;
	}

	// Get seconds left to refill a life. Can be used for notifications. Call only if canRefillLives() returns true.
	public double getRefillSecondsLeft(){
		return double.Parse (regenerationTimestamp)- getCurrentTimeInSeconds () ;
	}

	// Get seconds left to refill all lives. Can be used for notifications. Call only if canRefillLives() returns true.
	public double getFullRefillSecondsLeft(){

		double secondsToBeRefilled = 0;
		double livesToBeRefilled = getMaxNumberOfLives() - currentLives;

		if (livesToBeRefilled >0)
			secondsToBeRefilled = getRefillSecondsLeft ();

		if (livesToBeRefilled > 1) 
			for (int i = 0; i < livesToBeRefilled - 1; i++) 
				secondsToBeRefilled += LMConfig.REFILL_LIFE_SECONDS;

		return secondsToBeRefilled;
	}

	#endregion

	#region INTERNAL_API

	// Use this for initialization
	void Start () {

		// If its the first time, initialize permanent data store
		if (PlayerPrefs.GetInt (ID_FIRST_TIME) == 0) 
			firstTimeInit ();

		// Load local variables with correct values
		currentLives = PlayerPrefs.GetInt(ID_CURRENT_LIVES);
		extraLives = PlayerPrefs.GetInt(ID_EXTRA_LIVE_SLOTS);
		regenerationTimestamp = PlayerPrefs.GetString (ID_REGENERATION_TIMESTAMP);
		unlimitedTimestamp = PlayerPrefs.GetString (ID_UNLIMITED_TIMESTAMP);

		// Update interface with correct values
		updateUserInterface ();

		// If you dont have full lives, start the refill timer
		if (currentLives<getMaxNumberOfLives())
			InvokeRepeating("checkRegenerationTime", 0.0f, 1.0f);

		// If you have unlimited lives, start the unlimited lives timer
		if (isUnlimitedLives())
			InvokeRepeating("checkUnlimitedTime", 0.0f, 1.0f);
	}

	// If this is the first time, initialize all player preferences in permanent datastore
	void firstTimeInit(){
		Debug.Log ("First Time Init");
		PlayerPrefs.SetInt(ID_CURRENT_LIVES, LMConfig.BASIC_LIFE_SLOTS);
		PlayerPrefs.SetString (ID_REGENERATION_TIMESTAMP, "0");
		PlayerPrefs.SetString (ID_UNLIMITED_TIMESTAMP, "0");
		PlayerPrefs.SetString (ID_EXTRA_LIVE_SLOTS, "0");
		PlayerPrefs.SetInt (ID_FIRST_TIME, 1);
		PlayerPrefs.Save ();
	}

	// Resets all values in permanent store
	public void reset(){
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();
		Start ();
	}

    public Text currentLivesText;
    public Text textTimerText;

	void updateUserInterface(){
		// Update user interface
        currentLivesText.text = getCurrentLivesMsg();
        textTimerText.text = getTimeLeftMsg();

		//GameObject.Find ("TEXT_CURRENT_LIVES").GetComponent<Text> ().text = getCurrentLivesMsg();
		//GameObject.Find ("TEXT_TIMER").GetComponent<Text> ().text = getTimeLeftMsg();
	}

	// Returns current lives text
	string getCurrentLivesMsg(){
		if (isUnlimitedLives()) {
			return "∞";
		} else {
			return currentLives.ToString ();
		}
	}

	// Returns time left msg
	string getTimeLeftMsg(){
		if (isUnlimitedLives ())
			return secondsToTimeFormatter (getUnlimitedSecondsLeft ());

		if (currentLives == getMaxNumberOfLives ())
            return GameControll.instance.HashTableLang.GetUITextTutorial(LMConfig.TEXT_FULL_LIVES);
			//return LMConfig.TEXT_FULL_LIVES;
		else
			return secondsToTimeFormatter (getRefillSecondsLeft ());
	}

	// Format timer to appropriate format
	string secondsToTimeFormatter(double seconds){

		if (seconds < 3600) {// Show minutes and seconds format
			return string.Format("{0:0}:{1:00}", Mathf.Floor((float)seconds/60), Mathf.RoundToInt((float)seconds % 60));
		} else { // Show hours format
			return  Mathf.CeilToInt ((float)seconds / 3600).ToString () + " " + LMConfig.TEXT_HOURS_LEFT;
		} 
	}

	// Check if unlimited lives have ended.
	void checkUnlimitedTime(){

		// If unlmited lives have ended, stop the timer and reset permanent datastore values
		if (getUnlimitedSecondsLeft () <= 0) {
			unlimitedTimestamp = "0";
			PlayerPrefs.SetString (ID_UNLIMITED_TIMESTAMP, "0");
			PlayerPrefs.Save ();
			CancelInvoke ("checkUnlimitedTime"); 
		}

		updateUserInterface ();
	}
		
	// Check if a life has been refilled
	void checkRegenerationTime(){

		double refillSecondsLeft = getRefillSecondsLeft ();

		if (refillSecondsLeft <= 0) {
			int numberOfLivesToRestore = 1;
			numberOfLivesToRestore+=(int) Mathf.Abs ((float)refillSecondsLeft) / LMConfig.REFILL_LIFE_SECONDS;
			refillXLives (numberOfLivesToRestore);

			//Overwrite regeneration time stamp with seconds left
			int secondsLeft = (int)Mathf.Abs ((float)refillSecondsLeft) % LMConfig.REFILL_LIFE_SECONDS;
			if (secondsLeft > 0) {
				regenerationTimestamp =  (getCurrentTimeInSeconds () +  LMConfig.REFILL_LIFE_SECONDS - secondsLeft).ToString();
				PlayerPrefs.SetString (ID_REGENERATION_TIMESTAMP, regenerationTimestamp);
				PlayerPrefs.Save ();
			}
		}

		if (currentLives == getMaxNumberOfLives ()) {
			CancelInvoke ("checkRegenerationTime");
			regenerationTimestamp = "0";
			PlayerPrefs.SetString (ID_REGENERATION_TIMESTAMP, "0");
			PlayerPrefs.Save ();
		}
			updateUserInterface ();
	}

	// Set life regeneration timestamp in permanent datastore
	void setLifeRegenerationTimer(){
		regenerationTimestamp = (getCurrentTimeInSeconds () +  LMConfig.REFILL_LIFE_SECONDS).ToString();
		PlayerPrefs.SetString (ID_REGENERATION_TIMESTAMP, regenerationTimestamp);
		PlayerPrefs.Save ();
	}

	// Set unlmited lives timestamp in permanent datastore
	void setUnlimitedTimer(){
		unlimitedTimestamp = (getCurrentTimeInSeconds () +  LMConfig.UNLIMITED_LIVES_SECONDS).ToString();
		PlayerPrefs.SetString (ID_UNLIMITED_TIMESTAMP, unlimitedTimestamp);
		PlayerPrefs.Save ();
	}
		
	// Get seconds left to end unlimited lives
	double getUnlimitedSecondsLeft(){
		return double.Parse (unlimitedTimestamp) -  getCurrentTimeInSeconds () ;
	}

	// Get current time in seconds
	double  getCurrentTimeInSeconds(){
		var epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0,System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		return timestamp;
	}

	// Get maximum number of lives
	int getMaxNumberOfLives(){
		return LMConfig.BASIC_LIFE_SLOTS + extraLives;
	}

	// Returns true if you have unlimited lives
	bool isUnlimitedLives(){
		return getUnlimitedSecondsLeft () > 0;
	}

	#endregion
}
