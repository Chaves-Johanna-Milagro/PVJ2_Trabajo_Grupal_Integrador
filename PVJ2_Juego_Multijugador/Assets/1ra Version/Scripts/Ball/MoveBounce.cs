using System.Collections;
using UnityEngine;

public class MoveBounce : MonoBehaviour
{
    private float _speed = 8f;
    private float _limitX = 9f;      // Límite horizontal
    private float _limitY = 4.5f;    // Límite vertical
    private float _goalHeight = 3f;  // Altura del área de gol (zona central)

    private Rigidbody2D _rb;

    private MoveVertical _movePalet;

    private ScoreRight _scoreRight;
    private ScoreLeft _scoreLeft;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _movePalet = Object.FindAnyObjectByType<MoveVertical>();

        _scoreRight = Object.FindAnyObjectByType<ScoreRight>();
        _scoreLeft = Object.FindAnyObjectByType<ScoreLeft>();

    }

    void Start()
    {
        Launch();
    }

    void FixedUpdate()
    {
        Vector2 pos = _rb.position;

        // Rebote con el techo y suelo
        if (pos.y > _limitY || pos.y < -_limitY)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_rb.linearVelocity.y);
        }

        // Si se pasa del límite derecho
        if (pos.x > _limitX)
        {
            if (Mathf.Abs(pos.y) < _goalHeight / 2f)
            {
                // Gol derecho
                transform.position = Vector2.zero;
                _movePalet.ResetPos();
                Launch();
                Debug.Log("¡Gol derecha!");
                _scoreLeft.IncreasePoint();
                _scoreLeft.WinLevel();
                return;
            }
            else
            {
                // Rebote lateral superior/inferior (no gol)
                _rb.linearVelocity = new Vector2(-_rb.linearVelocity.x, _rb.linearVelocity.y);
            }
        }

        // Si se pasa del límite izquierdo
        if (pos.x < -_limitX)
        {
            if (Mathf.Abs(pos.y) < _goalHeight / 2f)
            {
                // Gol izquierdo
                transform.position = Vector2.zero;
                _movePalet.ResetPos();
                Launch();
                Debug.Log("¡Gol izquierda!");
                _scoreRight.IncreasePoint();
                _scoreRight.WinLevel();
                return;
            }
            else
            {
                // Rebote lateral superior/inferior (no gol)
                _rb.linearVelocity = new Vector2(-_rb.linearVelocity.x, _rb.linearVelocity.y);
            }
        }
    }

    private void Launch()
    {
        StartCoroutine(DelayLaunch());
    }

    private IEnumerator DelayLaunch()
    {
        _rb.linearVelocity = Vector2.zero; // pa que se quede quieta mientras espera
        yield return new WaitForSeconds(1f); // Espera 3 segundos antes de lanzar

        Vector2 dir = new Vector2(Random.value < 0.5f ? -1f : 1f, Random.Range(-0.5f, 0.5f)).normalized;
        _rb.linearVelocity = dir * _speed;
    }
}
