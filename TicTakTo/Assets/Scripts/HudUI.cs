using System;
using UnityEngine;

public class HudUI : MonoBehaviour
{
    [SerializeField] private GameObject _circleArrow;
    [SerializeField] private GameObject _crossArrow;

    void Start()
    {
        _circleArrow.SetActive(false);
        _crossArrow.SetActive(true);

        GameManager.Instance.OnTurnChanged += ChangeArrow;
    }

    private void ChangeArrow(SquareState currentTurn)
    {
        switch (currentTurn)
        { 
            case SquareState.None:
                _circleArrow.SetActive(false);
                _crossArrow.SetActive(false);
                break;
            case SquareState.Cross:
                _circleArrow.SetActive(false);
                _crossArrow.SetActive(true);                
                break;
            case SquareState.Circle:
                _circleArrow.SetActive(true);
                _crossArrow.SetActive(false);
                break;
            default:                
                throw new ArgumentOutOfRangeException($"{(int)currentTurn}");
                break;

        }
    }

}
