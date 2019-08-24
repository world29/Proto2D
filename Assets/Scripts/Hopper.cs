using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopper : MonoBehaviour
{
    public bool once = true;
    public Vector2 hoppingVelocity;
    public GameObject effectPrefab;

    private ComboSystem comboSystem;
    private JumpGauge jumpGauge;

    private void Start()
    {
        comboSystem = GameObject.Find("ComboText").GetComponent<ComboSystem>();
        jumpGauge = GameObject.Find("PlayerPanel").GetComponent<JumpGauge>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

             // エフェクト作成
            GameObject effect = Instantiate(effectPrefab) as GameObject;
            effect.transform.position = new Vector2( (transform.position.x + player.transform.position.x) / 2 ,transform.position.y);
            
            player.Hop(hoppingVelocity);

            if (player.incrementComboOnHop)
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
