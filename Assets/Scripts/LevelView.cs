using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] TMP_Text levelName;
    [SerializeField] GameObject lockIcon;
    private Button button;
    private int levelIndex;

    private void Awake()
    {
        button = GetComponent<Button>();
        lockIcon.SetActive(false);
    }

    public void Bind(int level, int maxLevel) // assigning listeners,text and lock icons in level buttons
    {
        levelIndex = level + 1;
        levelName.text = levelIndex.ToString();

        button.onClick.AddListener(() => ButtonMethod());

        if (maxLevel >= levelIndex)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
            lockIcon.SetActive(true);
        }
    }

    public void ButtonMethod() //listner of level buttons
    {
        SceneManager.LoadScene("GamePlay");
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
    }
}