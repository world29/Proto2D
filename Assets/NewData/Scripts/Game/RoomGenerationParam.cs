using UnityEngine;

namespace Assets.NewData.Scripts
{
    [CreateAssetMenu(fileName = "RoomGenerationParam", menuName = "Proto2D/RoomGenerationParam", order = 0)]
    public class RoomGenerationParam : ScriptableObject
    {
        public string[] roomNames;
    }
}