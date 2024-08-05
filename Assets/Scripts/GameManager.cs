using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Tooltip("Other than corner add prefab in ascending position according to their connectValue array's addition, add corner at last. ")]
    public GameObject[] piecePrefab;

    public Puzzle puzzle;

    public Transform generatePuzzle;

    public List<GameObject> endPoints = new();
    public GameObject levelCompletePanel;
    public LevelScriptable levelScriptable;
    public int currentLevel;
    public LevelData level;
    bool newLevel;
    public UnityEngine.Rendering.VolumeProfile volumeProfile;
    [HideInInspector] public UnityEngine.Rendering.Universal.Bloom bloom;

    private void Awake()
    {

        Instance = this;
        levelCompletePanel.SetActive(false);
    }

    void Start()
    {
        volumeProfile.TryGet(out bloom); //disable bloom effect
        bloom.active = false;
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        level = levelScriptable.levels[currentLevel - 1];

        GeneratePuzzle();

        generatePuzzle.localPosition = new Vector3((float)(-puzzle.width + 1) / 2, (float)(-puzzle.height + 1) / 2, 0);

        if (level.puzzle.winValue == 0)
            level.puzzle.winValue = GetWinValue();

        Shuffle();

        puzzle.currentValue = Sweep(); //assign random current value [connected puzzle value]
        puzzle.winValue = level.puzzle.winValue;  //to win value

        BlinkAnimation(false);
    }

    void GeneratePuzzle()  //generating puzzle according to re-visit of a level or a new level
    {
        puzzle = level.puzzle;

        puzzle.piece = new Piece[puzzle.width, puzzle.height];

        int[] defaultValues = { 0, 0, 0, 0 };

        if (level.pieceDatas.Length == 0) //meaning new level
        {
            newLevel = true;

            level.pieceDatas = new LevelData.PieceData[puzzle.height]; //creating new var


            for (int h = 0; h < puzzle.height; h++)
            {
                level.pieceDatas[h] = new LevelData.PieceData();
                level.pieceDatas[h].rotationValues = new Vector3[puzzle.width];
                level.pieceDatas[h].prefabs = new GameObject[puzzle.width];

                for (int w = 0; w < puzzle.width; w++)
                {
                    #region Restriction For Width

                    if (w == 0) defaultValues[3] = 0;
                    else defaultValues[3] = puzzle.piece[w - 1, h].connectValues[1];

                    if (w == puzzle.width - 1) defaultValues[1] = 0;
                    else defaultValues[1] = Random.Range(0, 2);

                    #endregion

                    #region restriction for height

                    if (h == 0) defaultValues[2] = 0;
                    else defaultValues[2] = puzzle.piece[w, h - 1].connectValues[0];

                    if (h == puzzle.height - 1) defaultValues[0] = 0;
                    else defaultValues[0] = Random.Range(0, 2);
                    #endregion

                    int totalValue = defaultValues[0] + defaultValues[1] + defaultValues[2] + defaultValues[3];

                    if (totalValue == 2 && defaultValues[0] != defaultValues[2])
                    {
                        totalValue = 5;
                    }

                    var piece = Instantiate(piecePrefab[totalValue], new Vector3(w, h, 0), Quaternion.identity, generatePuzzle);
                    level.pieceDatas[h].prefabs[w] = piecePrefab[totalValue]; //store values

                    if (totalValue == 1) endPoints.Add(piece);

                    while (piece.GetComponent<Piece>().connectValues
                        [0] != defaultValues[0] ||
                          piece.GetComponent<Piece>().connectValues[1] != defaultValues[1] ||
                          piece.GetComponent<Piece>().connectValues[2] != defaultValues[2] ||
                          piece.GetComponent<Piece>().connectValues[3] != defaultValues[3])
                    {
                        piece.GetComponent<Piece>().RotatePiece(0);
                    }

                    puzzle.piece[w, h] = piece.GetComponent<Piece>();
                }
            }
        }
        else
        {
            newLevel = false; //not a new level but a re-visit of a completed level

            for (int h = 0; h < puzzle.height; h++)
            {
                for (int w = 0; w < puzzle.width; w++)
                {
                    #region Restriction For Width

                    if (w == 0) defaultValues[3] = 0;
                    else defaultValues[3] = puzzle.piece[w - 1, h].connectValues[1];

                    if (w == puzzle.width - 1) defaultValues[1] = 0;
                    else defaultValues[1] = Random.Range(0, 2);

                    #endregion

                    #region restriction for height

                    if (h == 0) defaultValues[2] = 0;
                    else defaultValues[2] = puzzle.piece[w, h - 1].connectValues[0];

                    if (h == puzzle.height - 1) defaultValues[0] = 0;
                    else defaultValues[0] = Random.Range(0, 2);
                    #endregion

                    int totalValue = defaultValues[0] + defaultValues[1] + defaultValues[2] + defaultValues[3];

                    if (totalValue == 2 && defaultValues[0] != defaultValues[2])
                    {
                        totalValue = 5;
                    }

                    var piece = Instantiate(level.pieceDatas[h].prefabs[w], new Vector3(w, h, 0), Quaternion.identity, generatePuzzle);
                    if (totalValue == 1) endPoints.Add(piece);

                    puzzle.piece[w, h] = piece.GetComponent<Piece>();
                    puzzle.piece[w, h].transform.localEulerAngles = level.pieceDatas[h].rotationValues[w]; //assign rotations from scriptable
                }
            }
        }
        puzzle.winValue = level.puzzle.winValue;
    }

    void Shuffle() //shuffle rotation
    {
        if (newLevel)
        {
            foreach (var piece in puzzle.piece)
            {
                int j = Random.Range(0, 4);
                for (int i = 0; i < j; i++)
                {
                    piece.RotatePiece(0);
                }
            }
            Invoke(nameof(AddRotationValues), 0.5f);
        }
    }

    void AddRotationValues() //store rotation values in scriptable
    {
        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {
                level.pieceDatas[h].rotationValues[w] = puzzle.piece[w, h].transform.localEulerAngles;
            }
        }
    }

    public void BlinkAnimation(bool isPlay) //blink animation when complete level
    {
        Animator[] animator = generatePuzzle.gameObject.GetComponentsInChildren<Animator>();
        for (int i = 0; i < animator.Length; i++)
        {
            animator[i].enabled = isPlay;
        }
    }

    public int Sweep() //randomize rotation when generating a level
    {
        int value = 0;

        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {
                if (h != puzzle.height - 1)
                {
                    if (puzzle.piece[w, h].connectValues[0] == 1 && puzzle.piece[w, h + 1].connectValues[2] == 1)
                    {
                        value++;
                    }
                }

                if (w != puzzle.width - 1)
                {
                    if (puzzle.piece[w, h].connectValues[1] == 1 && puzzle.piece[w + 1, h].connectValues[3] == 1)
                    {
                        value++;
                    }
                }
            }
        }

        return value;
    }

    public int QuickSweep(int w, int h)
    {
        int value = 0;

        if (h != puzzle.height - 1)
        {
            if (puzzle.piece[w, h].connectValues[0] == 1 && puzzle.piece[w, h + 1].connectValues[2] == 1)
            {
                value++;
            }
        }

        if (w != puzzle.width - 1)
        {
            if (puzzle.piece[w, h].connectValues[1] == 1 && puzzle.piece[w + 1, h].connectValues[3] == 1)
            {
                value++;
            }
        }

        if (w != 0)
        {
            if (puzzle.piece[w, h].connectValues[3] == 1 && puzzle.piece[w - 1, h].connectValues[1] == 1)
            {
                value++;
            }
        }

        if (h != 0)
        {
            if (puzzle.piece[w, h].connectValues[2] == 1 && puzzle.piece[w, h - 1].connectValues[0] == 1)
            {
                value++;
            }
        }

        return value;
    }

    int GetWinValue()
    {
        int winValue = 0;
        foreach (var piece in puzzle.piece)
        {
            foreach (var item in piece.connectValues)
            {
                winValue += item;
            }
        }
        return winValue / 2;
    }

    public void HomeButton() //home button method
    {
        SceneManager.LoadScene(0);
    }

    public void NextButton() //next button method
    {
        if (currentLevel == 10)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
            SceneManager.LoadScene(1);
        }
    }

    public void RetryButton() //retry button method
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        SceneManager.LoadScene(1);
    }
}

[System.Serializable]
public class Puzzle
{
    public int winValue;
    public int currentValue;
    public int width;
    public int height;
    public Piece[,] piece;

    public class PieceCells
    {
        public Piece piece;
    }
}