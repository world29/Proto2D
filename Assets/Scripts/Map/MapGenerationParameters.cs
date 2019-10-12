using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Proto2D
{
    [Serializable, CreateAssetMenu(fileName = "MapGenerationParameters", menuName = "ScriptableObjects/CreateMapGenerationParameters")]
    public class MapGenerationParameters : ScriptableObject
    {
        // 乱数シード値
        [SerializeField]
        public int seed;

        // 物理シミュレーションのステップ数
        [SerializeField]
        public int simulationSteps;

        // 部屋生成数
        [SerializeField]
        public int roomGenerationCount;

        // 部屋を生成する空間の半径
        [SerializeField]
        public float roomGenerationAreaRadius;

        // 部屋の幅の平均値
        [SerializeField]
        public float roomGenerationSizeMeanX;

        // 部屋の高さの平均値
        [SerializeField]
        public float roomGenerationSizeMeanY;

        // 部屋の広さの分散
        [SerializeField]
        public float roomGenerationSizeSigma;

        // メイン部屋の最小サイズ
        [SerializeField]
        public float mainRoomThresholdX;
        [SerializeField]
        public float mainRoomThresholdY;

        // 廊下の幅
        [SerializeField]
        public float hallwayWidth;
    }
}
