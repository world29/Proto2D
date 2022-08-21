using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public interface IDamageable
    {
        bool TryDealDamage(float damageAmount);
    }
}
