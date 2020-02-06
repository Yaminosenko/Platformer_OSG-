using UnityEngine;

namespace FigmentGames
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public partial class EditablePolygonCollider : EnhancedMonoBehaviour
    {
        [SerializeField, HideInInspector] protected PolygonCollider2D _polygonCollider;
        public PolygonCollider2D polygonCollider
        {
            get
            {
                if (!_polygonCollider)
                    _polygonCollider = GetComponent<PolygonCollider2D>();

                return _polygonCollider;
            }
        }

        [Space(10)]
        [Header("TRIGGER")]
        [Tooltip("The offset of the collider shape.")]
        [SerializeField] private Vector2 _offset;
        public Vector2 offset { get { return _offset; } private set { _offset = value; } }
    }
}