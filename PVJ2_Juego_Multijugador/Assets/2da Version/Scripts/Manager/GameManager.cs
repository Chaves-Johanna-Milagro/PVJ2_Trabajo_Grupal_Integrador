using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private string _readyText = "isReady";
    private IPlayerUI[] _uiScripts;
    private bool _gameStarted = false;

    [Header("Configuración de Nivel")]
    [SerializeField] private bool _useMultipleSpawns = false; // True para Level 2

    private void Start()
    {
        // Evitamos que se sincronizen las escenas así cada jugador al ganar/perder va a la escena correcta
        PhotonNetwork.AutomaticallySyncScene = false;

        // Detectar automáticamente el nivel
        string sceneName = SceneManager.GetActiveScene().name;
        _useMultipleSpawns = (sceneName == "Level_2");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(_readyText))
        {
            bool isReady = (bool)changedProps[_readyText];
            Debug.Log($"[GameManager] Player {targetPlayer.ActorNumber} ({targetPlayer.NickName}) cambió su estado → isReady = {isReady}");
        }

        CheckIfBothReady();
    }

    private void CheckIfBothReady()
    {
        // No iniciar si ya se inició o si aún no hay dos jugadores
        if (_gameStarted) return;
        if (PhotonNetwork.PlayerList.Length < 2) return;

        bool p1Ready = false;
        bool p2Ready = false;

        var players = PhotonNetwork.PlayerList;

        if (players[0].CustomProperties.ContainsKey(_readyText))
            p1Ready = (bool)players[0].CustomProperties[_readyText];

        if (players[1].CustomProperties.ContainsKey(_readyText))
            p2Ready = (bool)players[1].CustomProperties[_readyText];

        // Ambos listos?
        if (p1Ready && p2Ready)
        {
            _gameStarted = true;
            StartCoroutine(StartGameSequence());
        }
    }

    private IEnumerator StartGameSequence()
    {
        Debug.Log("[GameManager] AMBOS jugadores listos → iniciando secuencia de partida!");

        // Paso 1: Activar UIs
        _uiScripts = FindObjectsOfType<MonoBehaviour>()
                        .OfType<IPlayerUI>()
                        .ToArray();

        foreach (var ui in _uiScripts)
        {
            ui.ActiveUI();
            Debug.Log("[GameManager] Activando UI del jugador...");
        }

        // Paso 2: Inicializar el pool (solo MasterClient)
        if (PhotonNetwork.IsMasterClient)
        {
            if (BallPoolManager.Instance != null)
            {
                BallPoolManager.Instance.InitializePool();
                Debug.Log("[GameManager] Inicializando pool de pelotas...");
            }
            else
            {
                Debug.LogError("[GameManager] BallPoolManager no encontrado!");
                yield break;
            }
        }

        // Paso 3: Esperar a que el pool esté listo
        float timeout = 5f;
        float elapsed = 0f;

        while (!BallPoolManager.Instance.IsPoolReady() && elapsed < timeout)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (!BallPoolManager.Instance.IsPoolReady())
        {
            Debug.LogError("[GameManager] Timeout esperando a que el pool esté listo");
            yield break;
        }

        // Paso 4: Iniciar sistema de juego según el nivel
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(0.5f);

            if (_useMultipleSpawns)
            {
                // Level 2: Sistema de spawn múltiple
                StartLevel2();
            }
            else
            {
                // Level 1: Una sola pelota
                StartLevel1();
            }
        }
    }

    private void StartLevel1()
    {
        // Spawnear una pelota normal en el centro
        BallPoolManager.Instance.SpawnBall("normal", Vector2.zero);
        Debug.Log("[GameManager] Level 1 iniciado - Pelota única spawneada");
    }

    private void StartLevel2()
    {
        // Buscar el BallSpawnerManager y activarlo
        BallSpawnerManager spawner = FindObjectOfType<BallSpawnerManager>();

        if (spawner != null)
        {
            spawner.StartSpawning();
            Debug.Log("[GameManager] Level 2 iniciado - Sistema de spawn múltiple activado");
        }
        else
        {
            Debug.LogError("[GameManager] BallSpawnerManager no encontrado en Level 2!");

            // Fallback: spawnear una pelota normal
            BallPoolManager.Instance.SpawnBall("normal", Vector2.zero);
        }
    }

    public override void OnLeftRoom()
    {
        // Detener spawns si estamos en Level 2
        if (_useMultipleSpawns)
        {
            BallSpawnerManager spawner = FindObjectOfType<BallSpawnerManager>();
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
        }

        // Limpiar el pool al salir de la sala
        if (BallPoolManager.Instance != null && PhotonNetwork.IsMasterClient)
        {
            BallPoolManager.Instance.ClearPool();
        }

        _gameStarted = false;
    }

    private void OnDestroy()
    {
        _gameStarted = false;
    }
}