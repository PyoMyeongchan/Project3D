using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TikTakTo ������ �����Ѵ�. => ����Ͻ� ���� / �ٽ� ���
/// ���ø����̼��� ���� �������� ������.
/// ����°� �������� ������ / ����°� �־������� �����
/// �������� ����ؿ� �����ϰ� �������Ѵ�.
/// 
/// KISS = Keep it simple, stupid
/// �ִ��� �ܼ��ϰ� ������
/// 
/// SRP = ���� å�� ��Ģ
/// </summary>

// ������� O X�� �����ϴ� ����
// ���带 �޸𸮿� ǥ��

public enum SquareState
{ 
    None,
    Cross,
    Circle    

}

// ������ ����
// O�� ���� / X�� ���� / ���º� / ���� ��
public enum GameOverState
{
    NotOver,
    Cross,
    Circle,
    Tie
}

public class GameManager : NetworkBehaviour
{
    // �̱���
    // GameManager�� ������ �����ϱ⿡ = �ٽ� ����̱⿡ �������� ����
    // �Է� ó��
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
        
    // �������� �Է¿� ���� ��û
    // �Է� : ��ǥ ��
    // ��� : X
    // RPC�� ��� RPC���̻� �߰��ؾ���!
    
    [Rpc(SendTo.Server)]
    public void RequestToPlayMarkRpc(int x, int y, SquareState localPlayerType)
    {
        // � Ŭ���̾�Ʈ�� ��û�� �� �ִ�.
        // �� �Է��� ��ȿ�Ѱ�?

        // Ŭ���̾�Ʈ���� �Է��ϸ� ������ �޴� ���� Ȯ���ϴ� �α�
        Logger.Info($"{nameof(RequestToPlayMarkRpc)} {x}, {y}, {localPlayerType}");

        if (false == isValidPlayMarker(x, y, localPlayerType))
        {
            return;
        }

        ChangeBoardStateRpc(x, y, localPlayerType);

        // ������ Ŭ���̾�Ʈ ����
        if (_currentTurnState.Value == SquareState.Cross)
        {
            _currentTurnState.Value = SquareState.Circle;  
        }
        else if (_currentTurnState.Value == SquareState.Circle)
        {
            _currentTurnState.Value = SquareState.Cross;
        };
        
               
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeBoardStateRpc(int x, int y, SquareState state)
    {
        _board[y, x] = state;
        OnBoardChanged?.Invoke(x, y, state);
    }


    private void Start()
    {
        // ���� �ٲ�
        _currentTurnState.OnValueChanged += (previousState, newState) =>
        {
            OnTurnChanged?.Invoke(newState);
        };

        // ����Ǿ����� �α�
        NetworkManager.Singleton.OnConnectionEvent += (networkManager, ConnectionEventData) =>
        {
            Logger.Info($"Client : {ConnectionEventData.ClientId} {ConnectionEventData.EventType}");

            if (networkManager.ConnectedClients.Count == 2)
            {
                StartGame();
            }
        };

        
    }

    private bool isValidPlayMarker(int x, int y, SquareState localPlayerType) 
    {
        return _gameOverState.Value != GameOverState.NotOver && _localPlayerType == _currentTurnState.Value && _board[y, x] == SquareState.None;
    }



    public void StartGame()
    {
        if (IsHost)
        {
            _localPlayerType = SquareState.Cross;
            // _currentTurnState�� ���� ������ �������Ը� �ִ�.
            _currentTurnState.Value = SquareState.Cross;
        }
        else 
        {
            _localPlayerType = SquareState.Circle;
        }        

    }
    

    // ���带 ��� ��Ÿ����
    // enum�� �̿��� 2�����迭��
    // ĭ�� ���´� 3�����̹Ƿ� ���������� �Ѵ�.
    // None, Cross, Circle
    private SquareState[,] _board = new SquareState[3,3];

    // ������ ����
    // ������ ���°� ����Ǿ��ٴ� ���� �˸� �̺�Ʈ ��ü�� �ʿ��ϴ�.
    // ������ ��ǥ / ���¸� ����
    public event Action<int, int, SquareState> OnBoardChanged;

    // GameOverUI ������
    public event Action<GameOverState> OnGameEnded;

    // HudUI ������
    public event Action<SquareState> OnTurnChanged;

    // ���� ��
    // �Է¿� ���� �� �����͸� ������ ����� ó���ؾ��Ѵ�.
    private NetworkVariable<SquareState> _currentTurnState = new();
    private NetworkVariable<GameOverState> _gameOverState = new();
    // �� Ŭ���̾�Ʈ�� ����
    private SquareState _localPlayerType = SquareState.None;


    public void ProcessInput(int x, int y)
    {
        // �������� �Է��� ��ȿ���� ��û
        if (_localPlayerType == _currentTurnState.Value)
        {
            RequestToPlayMarkRpc(x, y, _localPlayerType);
        }      

        /*
        // �¸� / ���ºεɽ� �� �̻� Ŭ���� �ȵǵ����ϱ�
        if (_gameOverState != GameOverState.NotOver)
        {
            return;
        }
        // �ѹ� �������� ��������
        if (_board[y, x] != SquareState.None)
        {
            return;
        }
        // �Է¹��� x,y ���� �־ Cross ���¸� ���忡 ���       
        _board[y, x] = _currentTurnState.Value;
        /*
        Logger.Info("������ ����");

        // ������ ���¸� �����ִ� Logger
        // ��ǥ���� ������ None �� Cross�� ������ �� �� ����
        // ex) (0,0)�� �����ٸ�
        // Cross(0,0)    None(1,0)    None(2,0)
        // None(0,1)     None(1,1)    None(2,1)
        // None(0,2)     None(1,2)    None(2,2)
        for (int line = 0; line < 3; ++line)
        {
            Logger.Info($"{_board[line, 0]}, {_board[line, 1]}, {_board[line, 2]}");
        }
        */
        // ���� ������ ��� �ؾ��ұ�
        // ����°� �������� ������ ������ GameManager�� ������� �ٽ� ����̱⿡ ������ �ٸ� Ŭ������ ����ؾ��Ѵ�!
        // ����� �ٸ� ��⿡�� �Ѱܾ��Ѵ�.
        // �̺�Ʈ / �������̽�, ������ ����

        // �ѹ� ������ ������ Ŭ���� �� �̻� �ȳ������� �ؾ��Ѵ�.

        /*
        // ������ ��ü���� ������ ���°� �ٲ���ٴ� ���� �����Ѵ�.
        OnBoardChanged?.Invoke(x, y, _currentTurnState.Value);


        _gameOverState= TestGameOver();

        // ���� ���� �� ���߰�
        if (_gameOverState != GameOverState.NotOver)
        {
            Logger.Info($"{_gameOverState} is winner");
            OnGameEnded?.Invoke(_gameOverState);
            return;
            
        }
        
        // ƽ����� OX�� �����ư��鼭 ������ ����
        // X ���� ������ �� �� O�� ������ �ؾ��Ѵ�.
        if (_currentTurnState.Value == SquareState.Cross)
        {
            _currentTurnState.Value = SquareState.Circle;
        }
        else if (_currentTurnState.Value == SquareState.Circle)
        {
            _currentTurnState.Value = SquareState.Cross;
        }
        
        OnTurnChanged?.Invoke(_currentTurnState.Value);
        */
    }


    GameOverState TestGameOver()
    {
        // �¸� ����
        // ���� Ȥ�� ���� Ȥ�� �밢������ ���� �������� ��Ÿ�����ٸ� �¸�
        // �¸��� ���̻� Ŭ�� �ȵǵ��� ���� (���� ����)

        // ���º� ����
        // �¸� ������ ���� �� �̻� �Է��� �ȵǾ��� �� ���º�
        // ���ºν� ���̻� Ŭ�� �ȵǵ��� ���� (���� ����)

        // �¸��� 3���� �̾��� �����տ� ������ �׾ ǥ��
        // ���ºν� �״�� ����

        // ���� �˻�
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

        // ���� �˻�
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

        // �밢�� �˻�
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

        // ���º�

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
