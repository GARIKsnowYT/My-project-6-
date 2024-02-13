using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZombieHorde : MonoBehaviour
{
    public string leaderTag = "Leader"; // ��� ������
    public float leaderDetectionRange = 10f; // ������ ����������� ������
    public float playerDetectionRange = 5f; // ������ ����������� ������
    public bool enableKinematic = true; // ������������� ��� ���������/���������� ���������� Rigidbody

    private Transform leader; // �����
    private Transform player; // �����

    private void Start()
    {
        // ������� ������ �� ����
        GameObject leaderObject = GameObject.FindGameObjectWithTag(leaderTag);
        if (leaderObject != null)
        {
            leader = leaderObject.transform;
        }
        else
        {
            Debug.LogError("Leader not found!");
        }

        // ������� ������
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // �������� ��� ��������� ���������� ��� ���� Rigidbody � �������� �������
        EnableRigidbodyKinematicRecursive(transform, enableKinematic);
    }

    private void Update()
    {
        // ���������� ���� ������ � �����
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // �������� ������ �� ��������� NavMeshAgent
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // ���������, ����� �� �����
                if (IsPlayerVisible(enemy))
                {
                    // ���� � ������
                    agent.SetDestination(player.position);
                }
                else
                {
                    // ��������� ���������� �� ������
                    if (Vector3.Distance(enemy.transform.position, leader.position) <= leaderDetectionRange)
                    {
                        // ���� � ������
                        agent.SetDestination(leader.position);
                    }
                }
            }
        }
    }

    // ��������, ����� �� ����� ��� ����������� �����
    bool IsPlayerVisible(GameObject enemy)
    {
        // ���������, ��������� �� ����� � �������� ������� ����������� �����
        return Vector3.Distance(enemy.transform.position, player.position) <= playerDetectionRange;
    }

    // �����, ���������� ��� ����������� ���������� ��� ���� Rigidbody � �������� �������
    void EnableRigidbodyKinematicRecursive(Transform parentTransform, bool enableKinematic)
    {
        // ���������� ��� �������� �������
        foreach (Transform child in parentTransform)
        {
            // �������� ��������� Rigidbody
            Rigidbody rb = child.GetComponent<Rigidbody>();

            // ���� ��������� Rigidbody ������������
            if (rb != null)
            {
                rb.isKinematic = enableKinematic; // �������� ��� ��������� ����������
            }
            // ���������� �������� ������� ��� ������� ��������� �������
            EnableRigidbodyKinematicRecursive(child, enableKinematic);
        }
    }

    // ����� ������ �����
    public void Die()
    {
        // ��������� ������ �����
        enabled = false;

        // ��������� ������������� �����
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        // ��������� ��������
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // ��������� ���������� ��� ���� Rigidbody � �������� �������
        EnableRigidbodyKinematicRecursive(transform, false);

        // ������� ������ ����� 10 ������
        Destroy(gameObject, 10f);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ZombieHorde))]
    public class ZombieHordeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // �������� ������ �� ������
            ZombieHorde zombieHorde = (ZombieHorde)target;

            // ������� ������ ��� ������ ������ ������
            if (GUILayout.Button("Kill Zombie"))
            {
                zombieHorde.Die();
            }
        }
    }
#endif
}
