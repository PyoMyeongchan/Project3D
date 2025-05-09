using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        GameManager.Instance.OnGameEnded += OnGameEnded;
        gameObject.SetActive(false);
    }

    private void OnGameEnded(GameOverState state)
    {
        gameObject.SetActive(true);
        if (state == GameOverState.Tie)
        {
            _text.text = "Tie!";
        }
        else if (state == GameOverState.Circle)
        {
            _text.text = "Circle is Winner";
        }
        else if (state == GameOverState.Cross)
        {
            _text.text = "Cross is Winner";
        }
    }

}
