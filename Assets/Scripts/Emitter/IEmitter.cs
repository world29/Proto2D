﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public interface IEmitter
    {
        // 発射する
        void Emit();

        // 速度プロパティ
        float Speed { get; set; }
    }
}
