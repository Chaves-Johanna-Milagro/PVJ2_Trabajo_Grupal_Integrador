using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BallSpawnerManager : MonoBehaviourPun
{
    [System.Serializable]
    public class SpawnConfig
    {
        [Tooltip("Tipo de pelota a spawnear (debe coincidir con BallPoolManager)")]
        public string ballType = "normal";

        [Tooltip("Tiempo mínimo entre spawns")]
        public float minSpawnTime = 2f;

        [Tooltip("Tiempo máximo entre spawns")]
        public float maxSpawnTime = 5f;

        [Tooltip("Número máximo de pelotas de este tipo activas simultáneamente")]
        public int maxActive = 3;
    }

    [Header("Configuración de Spawns")]
    [SerializeField] private SpawnConfig[] _spawnConfigs;

    [Header("Área de Spawn")]
    [SerializeField] private Vector2 _spawnAreaMin = new Vector2(-3f, -3f);
    [SerializeField] private Vector2 _spawnAreaMax = new Vector2(3f, 3f);

    [Header("Control")]
    [SerializeField] private bool _autoStart = true;

    private bool _isSpawning = false;

    // Inicia el sistema de spawn automático
    public void StartSpawning()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[BallSpawnerManager] Solo el MasterClient puede iniciar spawns");
            return;
        }

        if (_isSpawning)
        {
            Debug.LogWarning("[BallSpawnerManager] El spawning ya está activo");
            return;
        }

        if (!BallPoolManager.Instance.IsPoolReady())
        {
            Debug.LogError("[BallSpawnerManager] El pool no está inicializado");
            return;
        }

        _isSpawning = true;

        // Iniciar coroutine para cada tipo de pelota configurado
        foreach (var config in _spawnConfigs)
        {
            StartCoroutine(SpawnRoutine(config));
        }

        Debug.Log("[BallSpawnerManager] Sistema de spawn iniciado");
    }

    // Detiene el sistema de spawn
    public void StopSpawning()
    {
        _isSpawning = false;
        StopAllCoroutines();

        Debug.Log("[BallSpawnerManager] Sistema de spawn detenido");
    }

    // Corrutina que spawnea pelotas de un tipo específico
    private IEnumerator SpawnRoutine(SpawnConfig config)
    {
        Debug.Log($"[BallSpawnerManager] Iniciando spawn routine para '{config.ballType}'");

        while (_isSpawning)
        {
            // Esperar tiempo aleatorio
            float waitTime = Random.Range(config.minSpawnTime, config.maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (!_isSpawning) break; // Por si se detuvo mientras esperábamos

            // Verificar si no excedemos el máximo de pelotas activas de este tipo
            int currentCount = CountActiveBallsOfType(config.ballType);

            if (currentCount >= config.maxActive)
            {
                Debug.Log($"[BallSpawnerManager] Máximo de pelotas '{config.ballType}' alcanzado ({currentCount}/{config.maxActive}) - esperando...");
                continue;
            }

            // Generar posición aleatoria en el área de spawn
            Vector2 spawnPos = new Vector2(
                Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
            );

            // Spawnear pelota
            BallPoolManager.Instance.SpawnBall(config.ballType, spawnPos);

            Debug.Log($"[BallSpawnerManager] Spawneando pelota '{config.ballType}' en {spawnPos} (activas: {currentCount + 1}/{config.maxActive})");
        }

        Debug.Log($"[BallSpawnerManager] Spawn routine '{config.ballType}' detenida");
    }

    // Cuenta cuántas pelotas de un tipo específico están activas
    private int CountActiveBallsOfType(string ballType)
    {
        int count = 0;
        var activeBalls = BallPoolManager.Instance.GetActiveBalls();

        foreach (var ball in activeBalls)
        {
            if (ball == null || !ball.activeInHierarchy) continue;

            // Usar el componente BallData para identificar el tipo
            BallData ballData = ball.GetComponent<BallData>();
            if (ballData != null && ballData.GetBallType() == ballType)
            {
                count++;
            }
        }

        return count;
    }

    // Spawnea una pelota manual en una posición específica
    public void SpawnBallManual(string ballType, Vector2 position)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        BallPoolManager.Instance.SpawnBall(ballType, position);
    }

    // Spawnea una pelota manual en posición aleatoria
    public void SpawnBallRandom(string ballType)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Vector2 randomPos = new Vector2(
            Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
            Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
        );

        BallPoolManager.Instance.SpawnBall(ballType, randomPos);
    }

    // Para visualizar el área de spawn en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            (_spawnAreaMin.x + _spawnAreaMax.x) / 2f,
            (_spawnAreaMin.y + _spawnAreaMax.y) / 2f,
            0f
        );
        Vector3 size = new Vector3(
            _spawnAreaMax.x - _spawnAreaMin.x,
            _spawnAreaMax.y - _spawnAreaMin.y,
            0f
        );
        Gizmos.DrawWireCube(center, size);
    }
}