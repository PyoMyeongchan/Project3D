using UnityEngine;

/// <summary>
/// 보드의 상태를 애플리케이션에 출력한다.
/// GameManager 의존
/// </summary>
public class GameVisualManager : MonoBehaviour
{
    // 인스터싱을 위한 프리팹이 필요하다.
    // 프리팹을 생성하도록 어떻게 전달할 수 있을까
    // 위치값을 받아서 생성하자

    [SerializeField] private GameObject _crossMarkPrefab;
    [SerializeField] private GameObject _circleMarkPrefab;

    // 어떻게 프리팹을 인스턴싱 해야하는가.
    // 입력에서의 좌표값 transform을 받아서 나타날 수 있게
    // 언제 인스턴싱? → 보드의 상태가 바뀌었을 때 → GameManager의 상태가 변경됐을 때
    // 보드의 상태는 GameManager가 관리한다.

    // A가 B의 상태가 바뀌었는지 어떻게 아느냐
    // 1. 폴링(Polling) : 주기적으로 무언가를 검사하는 것
    // 폴링레이트 = 반응하는 속도
    // 장점 : 구현이 편하다.
    // 단점 : 쓸때없이 바쁘다.
    // 2. 옵저버 패턴(Observer) : B가 능동적으로 자신의 상태를 나한테 알려줬으면 좋겠다.
    // 이벤트 : 옵저버 패턴의 구현
    // 이벤트는 함수 담는 그릇이라고 이해하자
    // 단점 : 구독을 취소하기 어렵다.

    // 구독하기
    private void Start()
    {
        GameManager.Instance.OnBoardChanged += CreateMark;
    }

    private void CreateMark(int x, int y, SquareState squareState)
    {        
        // x, y : 보드의 좌표일뿐
        // 보드의 칸 좌표 => World Position으로 바꾸는 함수가 필요하다.
        switch (squareState)
        { 
            case SquareState.Cross:
                Instantiate(_crossMarkPrefab, GetWorldPositionFromCoordinate(x, y), Quaternion.identity);
                break;
            case SquareState.Circle:
                Instantiate(_circleMarkPrefab, GetWorldPositionFromCoordinate(x, y), Quaternion.identity);
                break;
            default:
                Logger.Error($"잘못된 값이 입력되었습니다. {(int)squareState}");
                break;
        
        }
    }

    // 좌표 변경하는 함수를 만들었다.
    private Vector2 GetWorldPositionFromCoordinate(int x, int y)
    {
        // (0,0) => Vector2(-3,3)
        // (1,0) => Vector2(0,3)
        // (2,0) => Vector2(3,3)

        // (0,1) => Vector2(-3,0)
        // (1,1) => Vector2(0,0)
        // (1,2) => Vector2(3,0)

        // (0,2) => Vector2(-3,-3)
        // (1,2) => Vector2(0,-3)
        // (2,2) => Vector2(3,-3)

        // x
        // 0 => -3 / 1 => 0 / 2 => 3
        int worldX = -3 + 3 * x;

        // y
        // 0 => 3 / 1 => 0 / 2 => -3
        int worldY = 3 + -3 * y; 
        
        return new Vector2(worldX, worldY);
        
        
    }

}