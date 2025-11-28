using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BallPoolManager : MonoBehaviourPun
{
    [System.Serializable]
    public class BallPoolConfig
    {
        public GameObject ballPrefab;
        public int poolSize = 3;
        public Vector3 scale = Vector3.one;
        [Tooltip("Identificador único para este tipo de pelota")]
        public string ballType = "normal";
    }

    [SerializeField] private BallPoolConfig[] _ballConfigs;

    // Diccionario de pools por tipo de pelota
    private Dictionary<string, Queue<GameObject>> _ballPools = new Dictionary<string, Queue<GameObject>>();
    private List<GameObject> _activeBalls = new List<GameObject>();

    private bool _isPoolInitialized = false;

    private static BallPoolManager _instance;
    public static BallPoolManager Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    /// Inicializa todos los pools configurados.
    public void InitializePool()
    {
        if (_isPoolInitialized)
        {
            Debug.LogWarning("[BallPoolManager] El pool ya fue inicializado");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[BallPoolManager] Solo el MasterClient puede inicializar el pool");
            return;
        }

        photonView.RPC("RPC_InitializePool", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_InitializePool()
    {
        if (_isPoolInitialized)
        {
            Debug.LogWarning("[BallPoolManager] RPC_InitializePool ignorado - pool ya inicializado");
            return;
        }

        Debug.Log("[BallPoolManager] RPC_InitializePool recibido");
        _isPoolInitialized = true;

        // SOLO el MasterClient instancia las pelotas
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[BallPoolManager] No es MasterClient - esperando pelotas del master");
            return;
        }

        Debug.Log("[BallPoolManager] Es MasterClient - creando pelotas...");

        foreach (var config in _ballConfigs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < config.poolSize; i++)
            {
                GameObject ball = PhotonNetwork.Instantiate(
                    config.ballPrefab.name,
                    new Vector3(1000, 1000, 0),
                    Quaternion.identity
                );

                ball.SetActive(false);
                pool.Enqueue(ball);

                // Establecer el tipo de pelota
                BallData ballData = ball.GetComponent<BallData>();
                if (ballData != null)
                {
                    ballData.SetBallType(config.ballType);
                }

                // Sincronizar la escala con todos los clientes
                PhotonView pv = ball.GetComponent<PhotonView>();
                if (pv != null)
                {
                    photonView.RPC("RPC_SetBallScale", RpcTarget.AllBuffered, pv.ViewID, config.scale);
                }

                Debug.Log($"[BallPoolManager] Pelota tipo '{config.ballType}' {i + 1}/{config.poolSize} creada (escala: {config.scale})");
            }

            _ballPools[config.ballType] = pool;
            Debug.Log($"[BallPoolManager] Pool '{config.ballType}' completo con {config.poolSize} pelotas");
        }

        Debug.Log($"[BallPoolManager]  Pool inicializado completamente con {_ballConfigs.Length} tipos de pelotas");
    }

    // Spawnea una pelota de un tipo específico

    /// <param name="ballType">Tipo de pelota (ej: "normal", "small", "large")</param>
    /// <param name="position">Posición de spawn</param>
    public void SpawnBall(string ballType, Vector2 position)
    {
        if (!_isPoolInitialized)
        {
            Debug.LogWarning("[BallPoolManager] El pool no ha sido inicializado aún");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[BallPoolManager] Solo el MasterClient puede spawnear pelotas");
            return;
        }

        photonView.RPC("RPC_SpawnBall", RpcTarget.AllBuffered, ballType, position);
    }

    // Sobrecarga para mantener compatibilidad con código existente (usa tipo "normal")
    public void SpawnBall(Vector2 position)
    {
        SpawnBall("normal", position);
    }

    [PunRPC]
    private void RPC_SpawnBall(string ballType, Vector2 position)
    {
        if (!_ballPools.ContainsKey(ballType))
        {
            Debug.LogError($"[BallPoolManager] Tipo de pelota '{ballType}' no existe en el pool");
            return;
        }

        GameObject ball;
        Queue<GameObject> pool = _ballPools[ballType];

        if (pool.Count > 0)
        {
            ball = pool.Dequeue();
        }
        else
        {
            // Si no hay pelotas disponibles, crear una nueva
            if (PhotonNetwork.IsMasterClient)
            {
                var config = System.Array.Find(_ballConfigs, c => c.ballType == ballType);

                if (config == null)
                {
                    Debug.LogError($"[BallPoolManager] Configuración para tipo '{ballType}' no encontrada");
                    return;
                }

                ball = PhotonNetwork.Instantiate(config.ballPrefab.name, position, Quaternion.identity);

                // Establecer el tipo de pelota
                BallData ballData = ball.GetComponent<BallData>();
                if (ballData != null)
                {
                    ballData.SetBallType(config.ballType);
                }

                // Sincronizar escala con todos los clientes
                PhotonView pv = ball.GetComponent<PhotonView>();
                if (pv != null)
                {
                    photonView.RPC("RPC_SetBallScale", RpcTarget.AllBuffered, pv.ViewID, config.scale);
                }

                Debug.Log($"[BallPoolManager] Pool de '{ballType}' vacío, creando nueva pelota");
            }
            else
            {
                Debug.LogWarning("[BallPoolManager] No hay pelotas disponibles y no soy MasterClient");
                return;
            }
        }

        ball.transform.position = position;
        ball.SetActive(true);
        _activeBalls.Add(ball);

        // Reiniciar velocidad de la pelota
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log($"[BallPoolManager] Pelota '{ballType}' spawneada en {position}");
    }

    // Devuelve una pelota al pool correspondiente según su tipo
    public void ReturnBall(GameObject ball)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[BallPoolManager] Solo el MasterClient puede devolver pelotas");
            return;
        }

        PhotonView pv = ball.GetComponent<PhotonView>();
        if (pv != null)
        {
            photonView.RPC("RPC_ReturnBall", RpcTarget.AllBuffered, pv.ViewID);
        }
    }

    [PunRPC]
    private void RPC_ReturnBall(int ballViewID)
    {
        PhotonView ballPV = PhotonView.Find(ballViewID);

        if (ballPV == null)
        {
            Debug.LogWarning($"[BallPoolManager] No se encontró PhotonView con ID {ballViewID}");
            return;
        }

        GameObject ball = ballPV.gameObject;

        if (_activeBalls.Contains(ball))
        {
            _activeBalls.Remove(ball);
        }

        // Determinar el tipo de pelota
        string ballType = GetBallType(ball);

        // Mover fuera de la vista antes de desactivar
        ball.transform.position = new Vector3(1000, 1000, 0);

        // Detener la física
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        ball.SetActive(false);

        // Devolver al pool correcto
        if (_ballPools.ContainsKey(ballType))
        {
            _ballPools[ballType].Enqueue(ball);
            Debug.Log($"[BallPoolManager] Pelota '{ballType}' devuelta al pool. Pool size: {_ballPools[ballType].Count}");
        }
    }

    // Determina el tipo de pelota basándose en el componente BallData
    private string GetBallTypeByScale(Vector3 scale)
    {
        // Primero intentar obtener el tipo desde BallData
        // Este método se mantiene como fallback por compatibilidad
        foreach (var config in _ballConfigs)
        {
            if (Vector3.Distance(scale, config.scale) < 0.01f)
            {
                return config.ballType;
            }
        }

        return "normal"; // Default
    }

    // Obtiene el tipo de una pelota desde su componente BallData
    private string GetBallType(GameObject ball)
    {
        BallData ballData = ball.GetComponent<BallData>();
        if (ballData != null)
        {
            return ballData.GetBallType();
        }

        // Fallback: usar escala
        return GetBallTypeByScale(ball.transform.localScale);
    }

    // Obtiene todas las pelotas activas
    public List<GameObject> GetActiveBalls()
    {
        return new List<GameObject>(_activeBalls);
    }

    // Obtiene el número de pelotas activas
    public int GetActiveBallCount()
    {
        return _activeBalls.Count;
    }

    // Limpia el pool cuando se sale de la sala
    public void ClearPool()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_ClearPool", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ClearPool()
    {
        foreach (var ball in _activeBalls)
        {
            if (ball != null)
            {
                ball.transform.position = new Vector3(1000, 1000, 0);
                ball.SetActive(false);

                string ballType = GetBallType(ball);
                if (_ballPools.ContainsKey(ballType))
                {
                    _ballPools[ballType].Enqueue(ball);
                }
            }
        }

        _activeBalls.Clear();
        _isPoolInitialized = false;

        Debug.Log("[BallPoolManager] Pool limpiado");
    }

    public bool IsPoolReady()
    {
        return _isPoolInitialized;
    }

    // RPC para sincronizar la escala de las pelotas entre todos los clientes
    [PunRPC]
    private void RPC_SetBallScale(int ballViewID, Vector3 scale)
    {
        PhotonView ballPV = PhotonView.Find(ballViewID);

        if (ballPV != null)
        {
            ballPV.transform.localScale = scale;
            Debug.Log($"[BallPoolManager] Escala de pelota {ballViewID} ajustada a {scale}");
        }
    }
}