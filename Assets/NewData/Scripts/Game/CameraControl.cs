using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class CameraControl : MonoBehaviour, ICameraControl
    {
        [SerializeField]
        private Camera target;

        [SerializeField]
        private RoomGenerator roomGenerator;

        [SerializeField]
        private float preloadDistance = 5f;

        [SerializeField]
        private Cinemachine.CinemachineConfiner confiner;

        [SerializeField]
        private BoxCollider2D confineCollider;

        void LateUpdate()
        {
            // 正射影カメラの上端が範囲を超えるとき、次の部屋を生成する。
            // (Camera.transform.position.y + Ortho.size) >= Confine2D.collider.size.y
            float cameraTop = target.transform.position.y + target.orthographicSize;
            if ((cameraTop + preloadDistance) >= confineCollider.size.y)
            {
                roomGenerator.GenerateNextRoom();
            }
        }

        // ICameraControl
        public void UpdateCameraConfine(float xMin, float yMin, float xMax, float yMax)
        {
            confineCollider.offset = new Vector2((xMin + xMax) / 2f, (yMin + yMax) / 2f);
            confineCollider.size = new Vector2(xMax - xMin, yMax - yMin);

            // invalidate
            confiner.InvalidatePathCache();
        }
    }
}
