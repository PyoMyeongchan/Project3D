using UnityEngine;

// 기능 : 마우스 버튼을 Unity 애플리케이션에 전달한다.
// 입력을 왔다는 것만 인식
public class GridPosition : MonoBehaviour
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;

    private void OnMouseDown()
    {
        Logger.Info($"({_x},{_y})클릭중");

        GameManager.Instance.ProcessInput(_x, _y);
    }
    
}


// 디버깅 하는 법
// 1. 디버거
// 2. 로그 - 실시간으로 반응하는 경우 더 유용
//  →Zlogger


// Build : 코드를 기반으로 실행가능한 프로그램을 만드는 것
// → 전처리 - 컴파일
// 전처리 과정 중 조건부 컴파일
// [Conditional("DEV_VER")] 적용
// 심볼 DEV_VER을 설정
// APPLY시 DEV_VER를 컴파일하겠다.
// 이를 없애고 한다면 컴파일 자체가 안된다.