
public static class GlobalStates
{
    private static bool isNewGame = true;
    public static bool IsTurotial {
        get { return isNewGame; }
    }
    private static int currentStage = 0;
    private static bool isfirstTimeDungeon = true;
    public static bool IsFirstTimeDungeon { get { return isfirstTimeDungeon; } }
    public static void firstTimeCloseTent() { isfirstTimeDungeon = false; }
    public static void tutorialFinished ()
    {
        isNewGame = false;
    }
    public static int CurrentStage
    {
        get { return currentStage; }
        set { currentStage = value; }
    }

    public static void stageCleared() {
        if (StageExplored < CurrentStage + 1)
            stageExplored = CurrentStage + 1;
    }
    private static int stageExplored = 0;
    public static int StageExplored
    {
        get { return stageExplored; }
    }

    private static bool isClosetChecked = false;

    public static bool IsClosetChecked { get { return isClosetChecked; } }
    public static void firstTimeCheckCloset() { isClosetChecked = true; }

    private static int numOfBattle = 0;

    public static int NumOfBattle { get { return numOfBattle; } }
    public static void battleFinished()
    {
        if (numOfBattle > 1)
            return;
        numOfBattle++;
    }
    public static void load(DataToSave data) 
    {
        numOfBattle = 2;
        isClosetChecked = data.isClosetChecked;
        stageExplored = data.stageExplored;
        isNewGame = false;
        isfirstTimeDungeon = false;
    }
    public static void reset() 
    {
        numOfBattle = 0;
        isClosetChecked = false;
        stageExplored = 0;
        isNewGame = true;
        isfirstTimeDungeon = true;
        currentStage = 0;
    }
}
