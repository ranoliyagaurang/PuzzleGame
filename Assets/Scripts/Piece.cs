using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IPointerClickHandler
{
    //Up, right, down, left -> Counting way, 1 = can connect, 0 = cannot connect
    public int[] connectValues;
    public float rotationSpeed;

    float rotation;

    public void OnPointerClick(PointerEventData eventData) //when user clicks on a piece
    {
        int difference = 0;
        difference -= GameManager.Instance.QuickSweep((int)transform.position.x, (int)transform.position.y);

        RotatePiece(rotationSpeed);

        difference += GameManager.Instance.QuickSweep((int)transform.position.x, (int)transform.position.y);

        GameManager.Instance.puzzle.currentValue += difference;

        if (GameManager.Instance.puzzle.currentValue == GameManager.Instance.puzzle.winValue)
        {
            GameManager.Instance.BlinkAnimation(true);
            GameManager.Instance.bloom.active = true;
            Invoke(nameof(EnableLevelCompletePanel), 1f);
        }
    }

    public void RotatePiece(float rotationSpeed) //rotate piece method using do tween
    {
        rotation += 90;
        transform.DORotate(new Vector3(0, 0, rotation), rotationSpeed);
        //transform.rotation = Quaternion.Euler(0, 0, rotation);
        RotateConnectValues();
    }

    public void RotateConnectValues() //connect values rotation
    {
        int lastValue = connectValues[0];

        for (int i = 0; i < connectValues.Length - 1; i++)
        {
            connectValues[i] = connectValues[i + 1];
        }
        connectValues[3] = lastValue;
    }

    public void EnableLevelCompletePanel()  //enable level complete panel when user completes a level
    {
        GameManager.Instance.levelCompletePanel.SetActive(true);
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int maxLevel = PlayerPrefs.GetInt("MaxLevel", 1);
        GameManager.Instance.bloom.active = false;
        if (currentLevel == maxLevel)
        {
            PlayerPrefs.SetInt("MaxLevel", maxLevel + 1);
        }
    }
}