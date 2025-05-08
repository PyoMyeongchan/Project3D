using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

/// <summary>
/// TikTakTo 게임을 진행한다. => 비즈니스 로직 / 핵심 모듈
/// 애플리케이션을 여러 계층으로 나눈다.
/// 입출력과 가까울수록 저수준 / 입출력과 멀어질수록 고수준
/// 저수준이 고수준에 의존하게 만들어야한다.
/// 
/// KISS = Keep it simple, stupid
/// 최대한 단순하게 만들어라
/// 
/// SRP = 단일 책임 원칙
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void ProcessInput(int x, int y)
    {
        Logger.Info("ProcessInput");
    }
}
