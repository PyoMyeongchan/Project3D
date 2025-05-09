using System;
using Unity.Netcode;
using UnityEngine;

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

// 순서대로 O X를 착수하는 게임
// 보드를 메모리에 표현

public enum SquareState
{ 
    None,
    Cross,
    Circle    

}

// 게임의 상태
// O가 승자 / X가 승자 / 무승부 / 게임 중
public enum GameOverState
{
    NotOver,
    Cross,
    Circle,
    Tie
}

public class GameManager : NetworkBehaviour
{
    // 싱글톤
    // GameManager로 게임을 진행하기에 = 핵심 모듈이기에 전역으로 관리
    // 입력 처리
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

    private void Start()
    {
        // 연결되었는지 로그
        NetworkManager.Singleton.OnConnectionEvent += (networkManager, ConnectionEventData) =>
        {
            Logger.Info($"OnConnection : {ConnectionEventData.EventType}");
        };
    }

    // 보드를 어떻게 나타낼지
    // enum을 이용한 2차원배열로
    // 칸의 상태는 3가지이므로 열거형으로 한다.
    // None, Cross, Circle
    private SquareState[,] _board = new SquareState[3,3];

    // 옵저버 패턴
    // 보드이 상태가 변경되었다는 것을 알릴 이벤트 객체가 필요하다.
    // 보드의 좌표 / 상태를 전달
    public event Action<int, int, SquareState> OnBoardChanged;

    // GameOverUI 옵저버
    public event Action<GameOverState> OnGameEnded;

    // HudUI 옵저버
    public event Action<SquareState> OnTurnChanged;

    // 현재 턴
    // 입력에 대해 이 데이터를 가지고 출력을 처리해아한다.
    private SquareState _currentTurnState = SquareState.Cross;
    private GameOverState _gameOverState = GameOverState.NotOver;


    public void ProcessInput(int x, int y)
    {
        // 승리 / 무승부될시 더 이상 클릭이 안되도록하기
        if (_gameOverState != GameOverState.NotOver)
        {
            return;
        }
        // 한번 눌러지면 못누르게
        if (_board[y, x] != SquareState.None)
        {
            return;
        }
        // 입력받은 x,y 값을 넣어서 Cross 상태를 보드에 기록       
        _board[y, x] = _currentTurnState;
        /*
        Logger.Info("보드의 상태");

        // 보드의 상태를 보여주는 Logger
        // 좌표값을 누르면 None → Cross로 변함을 알 수 있음
        // ex) (0,0)을 눌렀다면
        // Cross(0,0)    None(1,0)    None(2,0)
        // None(0,1)     None(1,1)    None(2,1)
        // None(0,2)     None(1,2)    None(2,2)
        for (int line = 0; line < 3; ++line)
        {
            Logger.Info($"{_board[line, 0]}, {_board[line, 1]}, {_board[line, 2]}");
        }
        */
        // 이제 생성을 어디서 해야할까
        // 입출력과 가까운것은 저수준 하지만 GameManager는 고수준의 핵심 모듈이기에 생성을 다른 클래스가 담당해야한다!
        // 출력을 다른 모듈에게 넘겨야한다.
        // 이벤트 / 인터페이스, 의존성 주입

        // 한번 눌러진 곳에는 클릭이 더 이상 안나오도록 해야한다.

        // 구독한 객체에게 보드의 상태가 바뀌었다는 것을 통지한다.
        OnBoardChanged?.Invoke(x, y, _currentTurnState);


        _gameOverState= TestGameOver();

        // 게임 종료 후 멈추게
        if (_gameOverState != GameOverState.NotOver)
        {
            Logger.Info($"{_gameOverState} is winner");
            OnGameEnded?.Invoke(_gameOverState);
            return;
            
        }

        // 틱택토는 OX가 번갈아가면서 나오는 게임
        // X 먼저 나오고 난 후 O가 나오게 해야한다.
        if (_currentTurnState == SquareState.Cross)
        {
            _currentTurnState = SquareState.Circle;
        }
        else if (_currentTurnState == SquareState.Circle)
        {
            _currentTurnState = SquareState.Cross;
        }

        OnTurnChanged?.Invoke(_currentTurnState);

    }


    GameOverState TestGameOver()
    {
        // 승리 판정
        // 세로 혹은 가로 혹은 대각선으로 같은 프리팹이 나타내졌다면 승리
        // 승리시 더이상 클릭 안되도록 설정 (게임 종료)

        // 무승부 판정
        // 승리 판정이 없이 더 이상 입력을 안되었을 때 무승부
        // 무승부시 더이상 클릭 안되도록 설정 (게임 종료)

        // 승리시 3개로 이어진 프리팹에 라인을 그어서 표시
        // 무승부시 그대로 유지

        // 가로 검사
        for (int y = 0; y < 3; ++y)
        {
            if (_board[y, 0] != SquareState.None && _board[y, 0] == _board[y, 1] && _board[y, 1] == _board[y, 2])
            {
                if (_board[y, 0] == SquareState.Cross)
                {
                    return GameOverState.Cross;
                }
                else if (_board[y, 0] == SquareState.Circle)
                {
                    return GameOverState.Circle;
                }

            }
        }

        // 세로 검사
        for (int x = 0; x < 3; ++x)
        {
            if (_board[0, x] != SquareState.None && _board[0, x] == _board[1, x] && _board[1, x] == _board[2, x])
            {
                if (_board[0, x] == SquareState.Cross)
                {
                    return GameOverState.Cross;
                }
                else if (_board[0, x] == SquareState.Circle)
                {
                    return GameOverState.Circle;
                }

            }
        }

        // 대각선 검사
        // 1. (0,0) == (1,1) == (2,2)
        if (_board[0, 0] != SquareState.None && _board[0, 0] == _board[1, 1] && _board[1, 1] == _board[2, 2])
        {
            if (_board[0, 0] == SquareState.Cross)
            {
                return GameOverState.Cross;
            }
            else if (_board[0, 0] == SquareState.Circle)
            {
                return GameOverState.Circle;
            }
        }

        // 2. (2,0) == (1,1) == (0,2)
        if (_board[2, 0] != SquareState.None && _board[2, 0] == _board[1, 1] && _board[1, 1] == _board[0, 2])
        {
            if (_board[2, 0] == SquareState.Cross)
            {
                return GameOverState.Cross;
            }
            else if (_board[2, 0] == SquareState.Circle)
            {
                return GameOverState.Circle;
            }
        }

        // 무승부

        for (int y = 0; y < 3; ++y)
        {
            for (int x = 0; x < 3; ++x)
            {
                if (_board[y, x] == SquareState.None)
                {
                    return GameOverState.NotOver;
                }
            }
        }

        return GameOverState.Tie;
    }


}
