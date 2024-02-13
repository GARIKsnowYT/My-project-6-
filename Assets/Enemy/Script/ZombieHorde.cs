using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZombieHorde : MonoBehaviour
{
    public string leaderTag = "Leader"; // Тег лидера
    public float leaderDetectionRange = 10f; // Радиус обнаружения лидера
    public float playerDetectionRange = 5f; // Радиус обнаружения игрока
    public bool enableKinematic = true; // Переключатель для включения/выключения кинематики Rigidbody

    private Transform leader; // Лидер
    private Transform player; // Игрок

    private void Start()
    {
        // Находим лидера по тегу
        GameObject leaderObject = GameObject.FindGameObjectWithTag(leaderTag);
        if (leaderObject != null)
        {
            leader = leaderObject.transform;
        }
        else
        {
            Debug.LogError("Leader not found!");
        }

        // Находим игрока
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Включаем или выключаем кинематику для всех Rigidbody в иерархии объекта
        EnableRigidbodyKinematicRecursive(transform, enableKinematic);
    }

    private void Update()
    {
        // Перебираем всех врагов в сцене
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // Получаем ссылку на компонент NavMeshAgent
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // Проверяем, виден ли игрок
                if (IsPlayerVisible(enemy))
                {
                    // Идем к игроку
                    agent.SetDestination(player.position);
                }
                else
                {
                    // Проверяем расстояние до лидера
                    if (Vector3.Distance(enemy.transform.position, leader.position) <= leaderDetectionRange)
                    {
                        // Идем к лидеру
                        agent.SetDestination(leader.position);
                    }
                }
            }
        }
    }

    // Проверка, виден ли игрок для конкретного врага
    bool IsPlayerVisible(GameObject enemy)
    {
        // Проверяем, находится ли игрок в пределах радиуса обнаружения врага
        return Vector3.Distance(enemy.transform.position, player.position) <= playerDetectionRange;
    }

    // Метод, включающий или выключающий кинематику для всех Rigidbody в иерархии объекта
    void EnableRigidbodyKinematicRecursive(Transform parentTransform, bool enableKinematic)
    {
        // Перебираем все дочерние объекты
        foreach (Transform child in parentTransform)
        {
            // Получаем компонент Rigidbody
            Rigidbody rb = child.GetComponent<Rigidbody>();

            // Если компонент Rigidbody присутствует
            if (rb != null)
            {
                rb.isKinematic = enableKinematic; // Включаем или выключаем кинематику
            }
            // Рекурсивно вызываем функцию для каждого дочернего объекта
            EnableRigidbodyKinematicRecursive(child, enableKinematic);
        }
    }

    // Метод смерти врага
    public void Die()
    {
        // Отключаем скрипт врага
        enabled = false;

        // Отключаем навигационный агент
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Отключаем аниматор
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Отключаем кинематику для всех Rigidbody в иерархии объекта
        EnableRigidbodyKinematicRecursive(transform, false);

        // Удаляем объект через 10 секунд
        Destroy(gameObject, 10f);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ZombieHorde))]
    public class ZombieHordeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Получаем ссылку на скрипт
            ZombieHorde zombieHorde = (ZombieHorde)target;

            // Создаем кнопку для вызова метода смерти
            if (GUILayout.Button("Kill Zombie"))
            {
                zombieHorde.Die();
            }
        }
    }
#endif
}
