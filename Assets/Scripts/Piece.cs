using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IPointerClickHandler
{
    //Up, right, down, left -> Counting way, 1 = can connect, 0 = cannot connect
    public int[] connectValues;
    public float rotationSpeed;

    public float rotation;

    public void SetRotation()
    {
        rotation = transform.localEulerAngles.z;
    }
    public void ManualRotate()
    {
        int angle = (int)rotation;
        while (angle > 0)
        {
            angle -= 90;
            RotateConnectValues();
        }
    }

    public void OnPointerClick(PointerEventData eventData) //when user clicks on a piece
    {
        int difference = 0;
        SoundManager.instance.PlaySFX(SoundManager.instance.rotatePiece);
        if (GameManager.Instance.puzzle.currentValue != GameManager.Instance.puzzle.winValue)
        {
            difference -= GameManager.Instance.QuickSweep((int)transform.localPosition.x, (int)transform.localPosition.y);

            RotatePiece(rotationSpeed);

            difference += GameManager.Instance.QuickSweep((int)transform.localPosition.x, (int)transform.localPosition.y);

            GameManager.Instance.puzzle.currentValue += difference;
        }

    }

    public void RotatePiece(float rotationSpeed) //rotate piece method using do tween
    {
        rotation += 90;
        transform.DOLocalRotate(new Vector3(0, 0, rotation), rotationSpeed).OnComplete(() =>
        {
            if (GameManager.Instance.puzzle.currentValue == GameManager.Instance.puzzle.winValue)
            {
                GameManager.Instance.BlinkAnimation(true);
                GameManager.Instance.bloom.active = true;
                SoundManager.instance.PlaySFX(SoundManager.instance.levelWin);
                Invoke(nameof(EnableLevelCompletePanel), 1f);
            }
        });

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