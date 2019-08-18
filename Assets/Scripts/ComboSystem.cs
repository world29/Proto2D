using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ComboSystem : MonoBehaviour
{
    [Header("コンボが継続する猶予時間")]
    public float timeToResetComboCounter = 3;

    [Header("表示するコンボ数の最小値")]
    public int minCountForShow = 2;

    [Header("カウンタが増えたときのスケールアニメーションの時間")]
    public float time = .2f;

    [SerializeField, Header("カウンタが増えたときのスケールアニメーションの値")]
    AnimationCurve curve;

    private int comboCounter;
    private float lastComboTime;

    private Text comboText;

    void Start()
    {
        comboText = GetComponent<Text>();

        ResetCombo();
    }

    void Update()
    {
        float elapsed = Time.time - lastComboTime;
        if (elapsed > timeToResetComboCounter)
        {
            ResetCombo();
        }
    }

    public void ResetCombo()
    {
        comboCounter = 0;

        UpdateUI();
    }

    public void IncrementCombo()
    {
        comboCounter++;
        lastComboTime = Time.time;

        UpdateUI();
        StartCoroutine(IncrementedEffect());
    }

    private void UpdateUI()
    {
        comboText.text = string.Format("{0} combo", comboCounter);

        if (comboCounter >= minCountForShow)
        {
            comboText.enabled = true;
        }
        else
        {
            comboText.enabled = false;
        }
    }

    IEnumerator IncrementedEffect()
    {
        float startTime = Time.timeSinceLevelLoad;
        float endTime = startTime + time;

        while (Time.timeSinceLevelLoad < endTime)
        {
            float percentage = (Time.timeSinceLevelLoad - startTime) / time;
            float value = curve.Evaluate(percentage);

            comboText.rectTransform.localScale = new Vector3(value, value);

            yield return new WaitForEndOfFrame();
        }

        comboText.rectTransform.localScale = Vector3.one;
    }
}
