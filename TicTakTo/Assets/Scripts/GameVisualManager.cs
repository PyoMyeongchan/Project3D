using UnityEngine;

/// <summary>
/// ������ ���¸� ���ø����̼ǿ� ����Ѵ�.
/// GameManager ����
/// </summary>
public class GameVisualManager : MonoBehaviour
{
    // �ν��ͽ��� ���� �������� �ʿ��ϴ�.
    // �������� �����ϵ��� ��� ������ �� ������
    // ��ġ���� �޾Ƽ� ��������

    [SerializeField] private GameObject _crossMarkPrefab;
    [SerializeField] private GameObject _circleMarkPrefab;

    // ��� �������� �ν��Ͻ� �ؾ��ϴ°�.
    // �Է¿����� ��ǥ�� transform�� �޾Ƽ� ��Ÿ�� �� �ְ�
    // ���� �ν��Ͻ�? �� ������ ���°� �ٲ���� �� �� GameManager�� ���°� ������� ��
    // ������ ���´� GameManager�� �����Ѵ�.

    // A�� B�� ���°� �ٲ������ ��� �ƴ���
    // 1. ����(Polling) : �ֱ������� ���𰡸� �˻��ϴ� ��
    // ��������Ʈ = �����ϴ� �ӵ�
    // ���� : ������ ���ϴ�.
    // ���� : �������� �ٻڴ�.
    // 2. ������ ����(Observer) : B�� �ɵ������� �ڽ��� ���¸� ������ �˷������� ���ڴ�.
    // �̺�Ʈ : ������ ������ ����
    // �̺�Ʈ�� �Լ� ��� �׸��̶�� ��������
    // ���� : ������ ����ϱ� ��ƴ�.

    // �����ϱ�
    private void Start()
    {
        GameManager.Instance.OnBoardChanged += CreateMark;
    }

    private void CreateMark(int x, int y, SquareState squareState)
    {        
        // x, y : ������ ��ǥ�ϻ�
        // ������ ĭ ��ǥ => World Position���� �ٲٴ� �Լ��� �ʿ��ϴ�.
        switch (squareState)
        { 
            case SquareState.Cross:
                Instantiate(_crossMarkPrefab, GetWorldPositionFromCoordinate(x, y), Quaternion.identity);
                break;
            case SquareState.Circle:
                Instantiate(_circleMarkPrefab, GetWorldPositionFromCoordinate(x, y), Quaternion.identity);
                break;
            default:
                Logger.Error($"�߸��� ���� �ԷµǾ����ϴ�. {(int)squareState}");
                break;
        
        }
    }

    // ��ǥ �����ϴ� �Լ��� �������.
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