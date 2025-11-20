using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    private Vector2 _startPos;

    private Vector2 _max = new Vector2(4f, 5f);
    private Vector2 _min = new Vector2(-4f, -5f);

    private float _speed = 12f;

    private Vector2 _targetPos;
    private bool _isMoving = false;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _startPos = transform.position;

        // Ajustar los límites basados en la posición de spawn
        _min += _startPos;
        _max += _startPos; 

        // Ajustar límites según extremos del sprite
        Vector2 spriteExtent = _sr.bounds.extents;

        _min.x += spriteExtent.x;
        _max.x -= spriteExtent.x;

        _min.y += spriteExtent.y;
        _max.y -= spriteExtent.y;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleInput();
    }

    // Movimiento dependiendo el tipo de plataforma
    void HandleInput()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    SetTarget(touch.position);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetTarget(Input.mousePosition);
            }
        }
    }

    // Movimiento hacia el click/touch
    void SetTarget(Vector3 inputPos)
    {
        Vector3 screenPos = new Vector3(inputPos.x, inputPos.y, -Camera.main.transform.position.z);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        _targetPos = new Vector2(
            Mathf.Clamp(worldPos.x, _min.x, _max.x),
            Mathf.Clamp(worldPos.y, _min.y, _max.y)
        );

        _isMoving = true;
    }

    // Usado para las fisicas
    void FixedUpdate()
    {
        if (!photonView.IsMine || !_isMoving) return;

        Vector2 currentPos = _rb.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, _targetPos, _speed * Time.fixedDeltaTime);

        _rb.MovePosition(newPos);

        if (Vector2.Distance(newPos, _targetPos) < 0.05f)
        {
            _isMoving = false;
        }
    }


}
