using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    int monsterHp = 5;
    float monsterAtk = 2;
    float monsterSpd = 1.0f;
    string strengthAttribute;
    string weakAttribute;
    bool isMoving = false;
    Vector3 _targetPosition;
    Animator _monsterAnimator;
    public ParticleSystem monsterDamageEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoving = true;
        _monsterAnimator = GetComponent<Animator>();
        _monsterAnimator.SetBool("IsMoving", isMoving);
        _targetPosition = transform.position + new Vector3(-11, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MonsterMove();

    }

    void MonsterMove()
    {
        if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
        {
            return;
        }
        Debug.Log("���� �̵���");

        transform.position = Vector3.Lerp(transform.position, Vector3.MoveTowards(transform.position, _targetPosition, monsterSpd * Time.deltaTime), 3.0f);
        Debug.Log("Ÿ�� ��ġ ����� " + _targetPosition);


    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("PlayerNoramalAttack"))
        {
            monsterDamageEffect.Play();
        }
    }
}
