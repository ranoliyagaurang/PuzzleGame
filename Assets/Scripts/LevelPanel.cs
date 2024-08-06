using UnityEngine;

public class LevelPanel : MonoBehaviour
{
    public static LevelPanel Instance;

    [SerializeField] LevelView levelPrefab;
    [SerializeField] Transform levelParent;
    [SerializeField] int totalLevel;  //total number of levels{10}
    public int currentLevel;
    public int maxLevel;

    LevelView levelView;

    private void Awake()
    {
        Application.targetFrameRate = 60;  //increase fps from 30[default] to 60
        Instance = this;
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);  //current level user is in
        maxLevel = PlayerPrefs.GetInt("MaxLevel", 1); //max level unlocked by user
    }

    void Start() //instantiate buttons according to current level and max unlocked level
    {
        for (int i = 0; i < totalLevel; i++)
        {
            levelView = Instantiate(levelPrefab, levelParent);
            levelView.Bind(i, maxLevel);
        }
    }
}