using System.Collections.Generic;
using UnityEngine;

public class CubeOrbitController : MonoBehaviour
{
    [Header("Параметры генерации (Awake)")]
    [Tooltip("Ссылка на префаб куба")]
    [SerializeField] private GameObject cubePrefab;
    
    [Tooltip("Количество кубиков")]
    [SerializeField] private int cubeCount = 6;
    
    [Tooltip("Включено — равномерно по кругу. Выключено — идут цепочкой друг за другом.")]
    [SerializeField] private bool evenlyDistributed = true;
    
    [Tooltip("Угловой шаг между кубами, если они идут цепочкой друг за другом")]
    [SerializeField] private float chainStepAngle = 20f;

    [Header("Параметры орбиты (Runtime)")]
    [Tooltip("Центр вращения. Если пусто — вращение вокруг этого объекта")]
    [SerializeField] private Transform centerPoint;
    
    [Tooltip("Радиус орбиты")]
    [SerializeField] private float radius = 5f;
    
    [Tooltip("Скорость вращения (градусов в секунду)")]
    [SerializeField] private float rotationSpeed = 60f;
    
    [Tooltip("Направление: включено — по часовой, выключено — против")]
    [SerializeField] private bool clockwise = true;

    private readonly List<Transform> spawnedCubes = new List<Transform>();
    private float currentAngleOffset = 0f;

    // Кубики создаются строго в Awake, как указано в задании
    private void Awake()
    {
        SpawnCubes();
    }

    private void SpawnCubes()
    {
        if (cubePrefab == null || cubeCount <= 0)
        {
            Debug.LogWarning("CubePrefab не назначен или CubeCount <= 0!");
            return;
        }

        for (int i = 0; i < cubeCount; i++)
        {
            GameObject instance = Instantiate(cubePrefab);
            spawnedCubes.Add(instance.transform);
        }
    }

    // Update вызывается каждый кадр — благодаря этому игра мгновенно 
    // реагирует на любые ползунки в Инспекторе во время работы игры (Runtime)
    private void Update()
    {
        if (spawnedCubes.Count == 0) return;

        // Направление и накопление угла
        float dir = clockwise ? -1f : 1f;
        currentAngleOffset += dir * rotationSpeed * Time.deltaTime;

        // Определение точки центра
        Vector3 center = (centerPoint != null) ? centerPoint.position : transform.position;

        // Расчет шага между кубами
        float step = evenlyDistributed ? (360f / spawnedCubes.Count) : chainStepAngle;

        // Обновление положения каждого куба по формуле окружности
        for (int i = 0; i < spawnedCubes.Count; i++)
        {
            if (spawnedCubes[i] == null) continue;

            float angle = currentAngleOffset + (i * step);
            float rad = angle * Mathf.Deg2Rad;

            // X = R * cos(a), Z = R * sin(a) — вращение в горизонтальной плоскости XZ
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
            spawnedCubes[i].position = center + offset;
        }
    }
}