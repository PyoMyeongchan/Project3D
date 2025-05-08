using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
