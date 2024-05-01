using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventListenerComponent))]
public class EnemyHUDController : MonoBehaviour
{
    [field:SerializeField] public GameObject HealthCanvas { get; private set; }
    [field: SerializeField] public TextMeshProUGUI AstralName { get; private set; }
    [field: SerializeField] public Slider HealthSlider { get; private set; }
    [field: SerializeField] public Slider RageSlider { get; private set; }
    [field: SerializeField] public Image RageFill{ get; private set; }
    [field: SerializeField] public Slider DamagedSlider { get; private set; }
    [field: SerializeField] public RectTransform UIMask { get; private set; }
    [field: SerializeField] public RectTransform HealthUI { get; private set; }
    [field: SerializeField] public RectTransform LeftBorder { get; private set; }
    [field: SerializeField] public RectTransform RightBorder { get; private set; }
    [field: SerializeField] public SO_EnemyStatus EnemyStatus { get; private set; }
    [SerializeField] private float _currentHealthValue;
    [SerializeField]private float _healthPreviousValue;
    float _time;
    private string _astralName;
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float _animationDuration;
    [Button("Debug Raise : Initialize Data")]
    public void OnLoadComplete()
    {
        EnemyStatus ??= GameObject.FindGameObjectWithTag("Astral Entity")?.GetComponent<Enemy>().StatusData;
        _astralName = EnemyStatus.Name;
        AstralName.text = _astralName;
        HealthSlider.maxValue = EnemyStatus.MaxHealth;
        DamagedSlider.maxValue = EnemyStatus.MaxHealth;
        HealthSlider.value = EnemyStatus.GetHealth();
        DamagedSlider.value = EnemyStatus.GetHealth();
        _healthPreviousValue = EnemyStatus.GetHealth();
        RageSlider.maxValue = EnemyStatus.BaseRageThreshold;
        RageSlider.value = EnemyStatus.F_RageMeter.value;
    }
    public void OnEnemyHealthChange()
    {
        if (EnemyStatus.GetHealth() < _healthPreviousValue)
        {
            _time = 0f;
            StopAllCoroutines();
            StartCoroutine(HealthReduced());
        }
        HealthSlider.value = EnemyStatus.GetHealth();
        _healthPreviousValue = EnemyStatus.GetHealth();
        RageSlider.value = EnemyStatus.F_RageMeter.value;
        if (EnemyStatus.B_Enraged) RageFill.color = Color.white;
        else RageFill.color = Color.yellow;

    }
    [Button("Debug Raise : Initalize UI")]
    public void OnExterminateInitialize()
    {
        StartCoroutine(HealthUIInitalizeAnimation());
    }
    [Button("Debug Raise : Reduce Health")]
    public void ReduceHealthTest(float damage)
    {
        EnemyStatus.AffectRage(damage);
        EnemyStatus.SetHealth(EnemyStatus.GetHealth() - damage);
    }
    [Button("Debug Raise : Reset Health")]
    public void ResetHealth()
    {
        EnemyStatus.SetHealth(EnemyStatus.MaxHealth);
        EnemyStatus.F_RageMeter.value = 0;
        DamagedSlider.value = EnemyStatus.GetHealth();
    }

    IEnumerator HealthReduced()
    {
        yield return StartCoroutine(Timer());
        StartCoroutine(ReduceHealth());
    }
    IEnumerator Timer()
    {
        while (_time < 0.35f)
        {
            _time += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator ReduceHealth()
    {
        float time = 0f;
        float speed;
        while (DamagedSlider.value > HealthSlider.value)
        {
            time += Time.deltaTime;
            speed = time * 1.25f;
            DamagedSlider.value = Mathf.Lerp(DamagedSlider.value, HealthSlider.value, speed);
            yield return null;
        }
    }
    IEnumerator HealthUIInitalizeAnimation()
    {
        yield return new WaitForSeconds(1f);
        HealthSlider.value = 0f;
        DamagedSlider.value = 0f;
        HealthCanvas.SetActive(true);
        UIMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        float time = 0f;
        float speed;
        float width;
        float uiHeight = HealthUI.rect.height;
        float borderWidth = LeftBorder.rect.width;

        while (UIMask.rect.width < HealthUI.rect.width)
        {
            time += Time.deltaTime;
            speed = _speedCurve.Evaluate(time / (_animationDuration / 2));
            width = Mathf.Lerp(1f, HealthUI.rect.width, speed);
            UIMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            LeftBorder.anchoredPosition = new Vector2(UIMask.rect.xMin -(borderWidth / 2), UIMask.anchoredPosition.y + (uiHeight / 2));
            RightBorder.anchoredPosition = new Vector2(UIMask.rect.xMax + (borderWidth / 2), UIMask.anchoredPosition.y+ (uiHeight / 2));
            yield return null;
        }
        time = 0f;
        while (HealthSlider.value < EnemyStatus.MaxHealth)
        {
            time += Time.deltaTime;
            speed = _speedCurve.Evaluate(time / (_animationDuration / 2));
            HealthSlider.value = Mathf.Lerp(0f, EnemyStatus.MaxHealth, speed);
            DamagedSlider.value = Mathf.Lerp(0f, EnemyStatus.MaxHealth, speed * 4);

            yield return null;
        }

    }
}

