using UnityEngine;
using UnityEngine.AI;

public class LeaderPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // ����� ��������������
    public float patrolSpeed = 2f; // �������� ��������������
    public float attackRange = 2f; // ������ �����
    public Collider visionZone; // ���� ���������

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private Transform player;
    private bool playerInVisionZone = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length == 0)
        {
            // ���� ����� �������������� �� ������, ���������� ��������� �����
            GenerateRandomPatrolPoints();
        }

        // ������� ������
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ������������� �� ������� ���������� ���� ���������
        visionZone = GetComponent<Collider>();
        visionZone.isTrigger = true;
    }

    void Update()
    {
        // ���� ����� � ������� ����� � � ���� ���������, �������
        if (playerInVisionZone && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
            return;
        }

        // ���� ����� � ���� ���������, ������� � ����
        if (playerInVisionZone)
        {
            agent.SetDestination(player.position);
        }
        // ���� ����� ������ ����� ��������������, ������������ � ���������
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            PatrolToNextPoint();
        }
    }

    // �������������� � ��������� �����
    void PatrolToNextPoint()
    {
        // ���� ��� ����� ��������������, ������� �� ������
        if (patrolPoints.Length == 0)
            return;

        // ������������� ����� ���������� ������� ����� ��������������
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        // ��������� � ��������� ����� ��������������
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // ��������� ��������� ����� ��������������
    void GenerateRandomPatrolPoints()
    {
        // ������� ��������� ����� ��������������
        int numberOfPoints = Random.Range(2, 6); // ���������� ��������� ���������� ����� (�� 2 �� 5)
        patrolPoints = new Transform[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            // ���������� ��������� ���������� ��� ����� �������������� � ������� 20 ������ �� ������
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 20f;
            randomPoint.y = transform.position.y; // ������������� Y ���������� ����� ��, ��� � ������
            patrolPoints[i] = new GameObject("PatrolPoint" + i).transform;
            patrolPoints[i].position = randomPoint;
        }
    }

    // ��������� ����� ������ � ���� ��������� ������
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVisionZone = true;
        }
    }

    // ��������� ������ ������ �� ���� ��������� ������
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVisionZone = false;
        }
    }

    // ����� ������
    void AttackPlayer()
    {
        // ��������, �� ������ �������� ����� ��� ��� ����� ������
    }
}
