using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public interface IProjectileEmitter
    {
        // 発射する
        void Emit(float speed, int elevation);
    }
}
