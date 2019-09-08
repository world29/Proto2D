using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopper : MonoBehaviour
{
    public bool once = true;
    public Vector2 hoppingVelocity;
    public GameObject effectPrefab;

    [Header("ホップでコンボ数が増える")]
    public bool incrementComboOnHop = false;

    private ComboSystem comboSystem;
    private JumpGauge jumpGauge;

    private void Start()
    {
        GameObject obj = GameObject.Find("ComboText");
        if (obj) {
            comboSystem = obj.GetComponent<ComboSystem>();
        }

        GameObject obj2 = GameObject.Find("PlayerPanel");
        if (obj2)
        {
            jumpGauge = obj2.GetComponent<JumpGauge>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                // エフェクト作成
                GameObject effect = Instantiate(effectPrefab) as GameObject;
                effect.transform.position = new Vector2((transform.position.x + player.transform.position.x) / 2, transform.position.y);

                player.Hop(hoppingVelocity);
            }

            if (comboSystem && incrementComboOnHop)
            {
                comboSystem.IncrementCombo();
                if (comboSystem.GetComboCount() % player.combosRequiredForBonusJump == 0)
                {
                    jumpGauge.IncrementJumpCount();
                }
            }

            if (once)
            {
                Destroy(gameObject);
            }
        }

    }
    
}
