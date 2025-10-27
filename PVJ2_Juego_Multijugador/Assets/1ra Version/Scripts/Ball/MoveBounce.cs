using UnityEngine;

public class MoveBounce : MonoBehaviour
{
    private float _speed = 8f;
    private float _limitX = 9f;      // L�mite horizontal
    private float _limitY = 4.5f;    // L�mite vertical
    private float _goalHeight = 3f;  // Altura del �rea de gol (zona central)

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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

        // Si se pasa del l�mite derecho
        if (pos.x > _limitX)
        {
            if (Mathf.Abs(pos.y) < _goalHeight / 2f)
            {
                // Gol derecho
                transform.position = Vector2.zero;
                Launch();
                Debug.Log("�Gol derecha!");
                return;
            }
            else
            {
                // Rebote lateral superior/inferior (no gol)
                _rb.linearVelocity = new Vector2(-_rb.linearVelocity.x, _rb.linearVelocity.y);
            }
        }

        // Si se pasa del l�mite izquierdo
        if (pos.x < -_limitX)
        {
            if (Mathf.Abs(pos.y) < _goalHeight / 2f)
            {
                // Gol izquierdo
                transform.position = Vector2.zero;
                Launch();
                Debug.Log("�Gol izquierda!");
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
        Vector2 dir = new Vector2(Random.value < 0.5f ? -1f : 1f, Random.Range(-0.5f, 0.5f)).normalized;
        _rb.linearVelocity = dir * _speed;
    }
}
