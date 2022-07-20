using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastController : MonoBehaviour
    {
        public LayerMask collisionMask;

        [SerializeField]
        public float skinWidth = .1f;

        [SerializeField]
        float dstBetweenRays = .25f;

        [HideInInspector]
        public int horizontalRayCount;
        [HideInInspector]
        public int verticalRayCount;

        [HideInInspector]
        public float horizontalRaySpacing;
        [HideInInspector]
        public float verticalRaySpacing;

        [HideInInspector]
        public BoxCollider2D boxCollider;
        public RaycastOrigins raycastOrigins;

        public virtual void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        public virtual void Start()
        {
            CalculateRaySpacing();
        }

        // スキンの内側の境界ボックスを計算する
        public Bounds ComputeInnerBounds()
        {
            // BoxCollider2D.bounds は FixedUpdate で更新されるため、Update で取得すると実際の位置とズレが生じる可能性がある。
            // そのため、BoxCollider2D.bounds を直接使用しない。
            // https://docs.unity3d.com/ScriptReference/Physics-autoSyncTransforms.html
            var bounds = new Bounds(transform.position, boxCollider.size);
            bounds.Expand(skinWidth * -2f);
            return bounds;
        }

        public void UpdateRaycastOrigins()
        {
            var bounds = ComputeInnerBounds();

            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public void CalculateRaySpacing()
        {
            var bounds = ComputeInnerBounds();

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }
    }
}
