using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class Controller2D : RaycastController
    {
        [Tooltip("上り下りできる坂道の最大角度 (deg)")]
        public float m_maxSlopeAngle = 60;

        [SerializeField, Tooltip("めり込み解決の有効化")]
        private bool overlapRecovery = false;

        [HideInInspector]
        public bool IsGrounded { get { return collisions.below; } }

        [HideInInspector]
        public CollisionInfo collisions;

        public override void Start()
        {
            base.Start();
        }

        public void Move(Vector2 moveAmount, bool facingRight = true, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();

            collisions.Reset();

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            HorizontalCollisions(ref moveAmount, facingRight);

            if (moveAmount.y != 0)
            {
                VerticalCollisions(ref moveAmount);
            }
            else
            {
                // 接地判定のため上下方向の移動がない場合は下方向のコリジョンを見る
                var dummyMove = new Vector2(0, -Time.deltaTime);
                VerticalCollisions(ref dummyMove);
            }

            // 壁掴みのチェック
            CheckLedge(ref moveAmount, facingRight);

            transform.Translate(moveAmount, standingOnPlatform ? Space.World : Space.Self);

            if (standingOnPlatform)
            {
                collisions.below = true;
            }

            RecoveryOverlap();
        }

        // めり込みの解消
        void RecoveryOverlap()
        {
            if (!overlapRecovery)
            {
                return;
            }

            var bounds = ComputeInnerBounds();

            var otherCollider = Physics2D.OverlapBox(bounds.center, bounds.size, 0, collisionMask);
            if (otherCollider && !otherCollider.CompareTag("Through"))
            {
                // Physics2D.autoSyncTransforms = false の場合にコライダの位置ズレを防ぐため、位置を強制的に同期する
                Physics2D.SyncTransforms();

                var colliderDistance = otherCollider.Distance(boxCollider);
                var moveAmount = colliderDistance.pointA - colliderDistance.pointB;

                transform.Translate(moveAmount);
            }
        }

        void HorizontalCollisions(ref Vector2 moveAmount, bool facingRight)
        {
            float directionX = facingRight ? 1f : -1f;
            if (moveAmount.x != 0)
            {
                directionX = Mathf.Sign(moveAmount.x);
            }

            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, hit ? Color.red : Color.blue);

                if (hit)
                {
                    if (hit.collider.CompareTag("Through"))
                    {
                        continue;
                    }

                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (i == 0 && slopeAngle <= m_maxSlopeAngle)
                    {
                        ClimbSlope(ref moveAmount, slopeAngle);
                    }

                    if (!collisions.climbingSlope && slopeAngle > m_maxSlopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        collisions.left = directionX == -1;
                        collisions.right = directionX == 1;

                        collisions.climbingWall = true;
                    }
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, hit ? Color.red : Color.blue);

                if (hit)
                {
                    if (hit.collider.CompareTag("Through"))
                    {
                        if (directionY == 1 || hit.distance == 0)
                        {
                            continue;
                        }

                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }

                        /* 下入力での床すり抜けを無効化
                        if (m_input.y == -1)
                        {
                            collisions.fallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", .5f);
                            continue;
                        }
                        */
                    }

                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
        }

        void CheckLedge(ref Vector2 moveAmount, bool facingRight)
        {
            // 体の位置と頭上から真横にレイを飛ばし、
            // 頭上のみヒットしなければ、崖に差し掛かったと判定する。
            RaycastHit2D hitBody = default, hitAboveHead;
            {
                float rayLength = 0.2f;
                Vector2 rayDir = facingRight ? Vector2.right : Vector2.left;

                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = facingRight ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                    hitBody = Physics2D.Raycast(rayOrigin, rayDir, rayLength, collisionMask);

                    Debug.DrawRay(rayOrigin, rayDir * rayLength, hitBody ? Color.red : Color.blue);

                    if (hitBody)
                    {
                        if (hitBody.collider.CompareTag("Through"))
                        {
                            continue;
                        }

                        break;
                    }
                }

                {
                    Vector2 rayOrigin = new Vector2(
                        facingRight ? raycastOrigins.bottomRight.x : raycastOrigins.bottomLeft.x,
                        raycastOrigins.topRight.y + horizontalRaySpacing);

                    hitAboveHead = Physics2D.Raycast(rayOrigin, rayDir, rayLength, collisionMask);

                    Debug.DrawRay(rayOrigin, rayDir * rayLength, hitAboveHead ? Color.red : Color.blue);
                }
            }

            if (hitBody && !hitAboveHead)
            {
                // 崖の地面の高さを取得して、そこにキャラクターを移動させる
                float offset = facingRight ? 0.1f : -0.1f;
                float rayLength = raycastOrigins.topRight.y - raycastOrigins.bottomRight.y;
                Vector2 rayDir = Vector2.down;

                Vector2 rayOrigin = new Vector2(hitBody.point.x + offset, raycastOrigins.topRight.y + horizontalRaySpacing);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, rayDir * rayLength, hit ? Color.red : Color.blue);
                if (hit)
                {
                    collisions.ledgeCorner = new Vector2(hitBody.point.x, hit.point.y);
                    collisions.ledgeClimbPosition = hit.point;

                    collisions.touchingLedge = true;
                }
            }
        }

        void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbAmountY)
            {
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                moveAmount.y = climbAmountY;
                collisions.below = true;
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
            }
        }

        void DescendSlope(ref Vector2 moveAmount)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            Debug.DrawRay(rayOrigin, -Vector2.up * Mathf.Infinity, hit ? Color.red : Color.blue);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Max(moveDistance, .1f))
                        {
                            float descendAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendAmountY;

                            collisions.below = true;
                            collisions.descendingSlope = true;
                        }
                    }
                }
            }
        }

        void ResetFallingThroughPlatform()
        {
            collisions.fallingThroughPlatform = false;
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public bool climbingSlope;
            public bool descendingSlope;
            public float slopeAngle, slopeAngleOld;

            public bool climbingWall;

            public bool fallingThroughPlatform;

            public bool touchingLedge;
            public Vector2 ledgeCorner; // 崖のふち
            public Vector2 ledgeClimbPosition; // 崖登り後の位置

            public void Reset()
            {
                above = below = false;
                left = right = false;

                climbingSlope = false;
                descendingSlope = false;

                slopeAngleOld = slopeAngle;
                slopeAngle = 0;

                climbingWall = false;

                touchingLedge = false;
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider.enabled)
            {
                var gizmoColor = Gizmos.color;

                Gizmos.color = new Color(1, 1, 1, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);

                Bounds bounds = collider.bounds;
                bounds.Expand(skinWidth * -2);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(bounds.center, bounds.size);

                Gizmos.color = gizmoColor;
            }
        }
    }
}
