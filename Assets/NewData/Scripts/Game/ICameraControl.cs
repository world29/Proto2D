using UnityEngine;

namespace Assets.NewData.Scripts
{
    public interface ICameraControl
    {
        void UpdateCameraConfine(float xMin, float yMin, float xMax, float yMax);
    }
}
