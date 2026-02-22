using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private MovementType _movementType = MovementType.Horizontal;
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _speed = 2f;

    [Header("Options")]
    [SerializeField] private bool _startAtEnd = false;
    [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private bool _moveObjects = true; // Mover objetos que estén encima

    [Header("Gizmos")]
    [SerializeField] private Color _gizmoColor = Color.yellow;
    [SerializeField] private float _gizmoSize = 0.3f;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _lastPosition;
    private float _progress = 0f;
    private bool _movingForward = true;

    public enum MovementType
    {
        Horizontal,
        Vertical
    }

    private void Start()
    {
        _startPosition = transform.position;
        _lastPosition = _startPosition;

        _endPosition = _movementType == MovementType.Horizontal
            ? _startPosition + Vector3.right * _distance
            : _startPosition + Vector3.up * _distance;

        if (_startAtEnd)
        {
            transform.position = _endPosition;
            _lastPosition = _endPosition;
            _progress = 1f;
            _movingForward = false;
        }
    }

    private void Update()
    {
        // Calcular progreso
        float step = _speed * Time.deltaTime / _distance;

        if (_movingForward)
        {
            _progress += step;
            if (_progress >= 1f)
            {
                _progress = 1f;
                _movingForward = false;
            }
        }
        else
        {
            _progress -= step;
            if (_progress <= 0f)
            {
                _progress = 0f;
                _movingForward = true;
            }
        }

        // Aplicar curva de movimiento
        float curvedProgress = _movementCurve.Evaluate(_progress);

        // Mover plataforma
        Vector3 newPosition = Vector3.Lerp(_startPosition, _endPosition, curvedProgress);
        transform.position = newPosition;

        _lastPosition = transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_moveObjects) return;

        // Mover objetos que estén encima de la plataforma
        Vector3 movement = transform.position - _lastPosition;
        collision.transform.position += movement;
    }

    private void OnDrawGizmos()
    {
        Vector3 startPos = Application.isPlaying ? _startPosition : transform.position;
        Vector3 endPos = _movementType == MovementType.Horizontal
            ? startPos + Vector3.right * _distance
            : startPos + Vector3.up * _distance;

        // Línea de recorrido
        Gizmos.color = _gizmoColor;
        Gizmos.DrawLine(startPos, endPos);

        // Puntos de inicio y fin
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos, _gizmoSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPos, _gizmoSize);

        // Dirección
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 arrowPos = startPos + direction * (_distance * 0.5f);
        DrawArrow(arrowPos, direction, _gizmoColor);

        // Dibujar plataforma en puntos clave
        if (!Application.isPlaying)
        {
            DrawPlatformPreview(startPos, 0.3f);
            DrawPlatformPreview(endPos, 0.3f);
            DrawPlatformPreview(Vector3.Lerp(startPos, endPos, 0.5f), 0.2f);
        }
    }

    private void DrawPlatformPreview(Vector3 position, float alpha)
    {
        // Obtener el tamaño del collider para dibujar la plataforma
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Color previewColor = _gizmoColor;
            previewColor.a = alpha;
            Gizmos.color = previewColor;

            Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.1f);
            Gizmos.DrawCube(position, size);
        }
    }

    private void DrawArrow(Vector3 pos, Vector3 direction, Color color)
    {
        Gizmos.color = color;
        float arrowHeadLength = 0.3f;
        float arrowHeadAngle = 25f;

        Gizmos.DrawRay(pos, direction * arrowHeadLength * 2);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Gizmos.DrawRay(pos + direction * arrowHeadLength * 2, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction * arrowHeadLength * 2, left * arrowHeadLength);
    }
}