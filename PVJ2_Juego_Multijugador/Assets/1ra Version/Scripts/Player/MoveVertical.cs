using UnityEngine;

public class MoveVertical : MonoBehaviour
{
    private float _speed = 10f;
    private float _limitY = 5f;
    private float _extremesY;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Detecta los limites del sprite 
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _extremesY = sr.bounds.extents.y;
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Vertical"); // W/S o ↑/↓
        _rb.linearVelocity = new Vector2(0f, input * _speed);

        // Limitar posición vertical
        Vector2 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -_limitY + _extremesY, _limitY - _extremesY);
        transform.position = pos;
    }
}
