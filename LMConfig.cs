public static class LMConfig {

	// Basic life slots to start with. 
	public const int BASIC_LIFE_SLOTS = 5;

    // How long until you receive a +1 life refill (in seconds)
    public const int REFILL_LIFE_SECONDS = 1200;

    // How long unlimited lives will last for (in seconds)
    public const int UNLIMITED_LIVES_SECONDS = 7200;

	// How many extra life slots can you buy (permanent life slots on top of your basic life slots)
	public const int MAX_EXTRA_LIFE_SLOTS = 2;

	// Text to be displayed in TEXT_TIMER text if you have full lives
	public const string TEXT_FULL_LIVES = "Full";

	// Text to be displayed in TEXT_TIMER if you more than 1 hour left to refill life or for unlimited lives to end
	public const string TEXT_HOURS_LEFT = "hrs";

}
