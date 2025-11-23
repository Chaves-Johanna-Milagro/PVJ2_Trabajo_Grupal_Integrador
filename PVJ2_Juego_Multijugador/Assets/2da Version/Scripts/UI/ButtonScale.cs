using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 _originalScale;
    private Vector3 _pressedScale;
    private Vector3 _hoverScale;

    private Vector3 _targetScale;
    private float _speed = 10f;

    private bool _isHovering = false;
    private void Start()
    {
        _originalScale = transform.localScale;
        _pressedScale = _originalScale * 1.1f; // Tamaño al presionar
        _hoverScale = _originalScale * 1.1f;   // Tamaño al pasar el cursor

        _targetScale = _originalScale;
    }
    private void Update()
    {
        // Interpolación suave
        transform.localScale = Vector3.Lerp( transform.localScale, _targetScale, Time.deltaTime * _speed);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        _targetScale = _hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        _targetScale = _originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetScale = _pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _targetScale = _isHovering ? _hoverScale : _originalScale;
    }
}
