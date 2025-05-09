using UnityEngine;

// ��� : ���콺 ��ư�� Unity ���ø����̼ǿ� �����Ѵ�.
// �Է��� �Դٴ� �͸� �ν�
public class GridPosition : MonoBehaviour
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;

    private void OnMouseDown()
    {
        Logger.Info($"({_x},{_y})Ŭ����");

        GameManager.Instance.ProcessInput(_x, _y);
    }
    
}


// ����� �ϴ� ��
// 1. �����
// 2. �α� - �ǽð����� �����ϴ� ��� �� ����
//  ��Zlogger


// Build : �ڵ带 ������� ���డ���� ���α׷��� ����� ��
// �� ��ó�� - ������
// ��ó�� ���� �� ���Ǻ� ������
// [Conditional("DEV_VER")] ����
// �ɺ� DEV_VER�� ����
// APPLY�� DEV_VER�� �������ϰڴ�.
// �̸� ���ְ� �Ѵٸ� ������ ��ü�� �ȵȴ�.