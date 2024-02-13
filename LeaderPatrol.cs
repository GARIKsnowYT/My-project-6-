using UnityEngine;
using UnityEngine.AI;

public class LeaderPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // Точки патрулирования
    public float patrolSpeed = 2f; // Скорость патрулирования
    public float attackRange = 2f; // Радиус атаки
    public Collider visionZone; // Зона видимости

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private Transform player;
    private bool playerInVisionZone = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length == 0)
        {
            // Если точки патрулирования не заданы, генерируем случайные точки
            GenerateRandomPatrolPoints();
        }

        // Находим игрока
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Подписываемся на события коллайдера зоны видимости
        visionZone = GetComponent<Collider>();
        visionZone.isTrigger = true;
    }

    void Update()
    {
        // Если игрок в радиусе атаки и в зоне видимости, атакуем
        if (playerInVisionZone && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
            return;
        }

        // Если игрок в зоне видимости, следуем к нему
        if (playerInVisionZone)
        {
            agent.SetDestination(player.position);
        }
        // Если лидер достиг точки патрулирования, перемещаемся к следующей
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            PatrolToNextPoint();
        }
    }

    // Патрулирование к следующей точке
    void PatrolToNextPoint()
    {
        // Если нет точек патрулирования, выходим из метода
        if (patrolPoints.Length == 0)
            return;

        // Устанавливаем пункт назначения текущей точке патрулирования
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        // Переходим к следующей точке патрулирования
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // Генерация случайных точек патрулирования
    void GenerateRandomPatrolPoints()
    {
        // Создаем случайные точки патрулирования
        int numberOfPoints = Random.Range(2, 6); // Генерируем случайное количество точек (от 2 до 5)
        patrolPoints = new Transform[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            // Генерируем случайные координаты для точек патрулирования в радиусе 20 единиц от лидера
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 20f;
            randomPoint.y = transform.position.y; // Устанавливаем Y координату такой же, как у лидера
            patrolPoints[i] = new GameObject("PatrolPoint" + i).transform;
            patrolPoints[i].position = randomPoint;
        }
    }

    // Обработка входа игрока в зону видимости лидера
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVisionZone = true;
        }
    }

    // Обработка выхода игрока из зоны видимости лидера
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVisionZone = false;
        }
    }

    // Атака игрока
    void AttackPlayer()
    {
        // Например, вы можете добавить здесь код для атаки игрока
    }
}
