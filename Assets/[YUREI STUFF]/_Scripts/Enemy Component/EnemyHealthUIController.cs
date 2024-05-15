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
    [field: Header("Animation Properties")]
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float _animationDuration;
    public bool debug;
    [ShowIf(nameof(debug),true)][SerializeField] private SO_EnemyStatus _enemyStatus;
    private float _currentHealthValue;
    private float _healthPreviousValue;
    float _time;
    private string _astralName;

    [Button("Debug Raise : Initialize Data")]
    public void OnLoadComplete()
    {
        _enemyStatus ??= GameObject.FindGameObjectWithTag("Astral Entity")?.GetComponent<Enemy>().StatusData;
        _astralName = _enemyStatus.Name;
        AstralName.text = _astralName;
        HealthSlider.maxValue = _enemyStatus.MaxHealth;
        DamagedSlider.maxValue = _enemyStatus.MaxHealth;
        HealthSlider.value = _enemyStatus.GetHealth();
        DamagedSlider.value = _enemyStatus.GetHealth();
        _healthPreviousValue = _enemyStatus.GetHealth();
        RageSlider.maxValue = _enemyStatus.BaseRageThreshold;
        RageSlider.value = _enemyStatus.F_RageMeter.value;
    }
    public void OnEnemyHealthChange()
    {
        if (_enemyStatus.GetHealth() < _healthPreviousValue)
        {
            _time = 0f;
            StopAllCoroutines();
            StartCoroutine(HealthReduced());
        }
        HealthSlider.value = _enemyStatus.GetHealth();
        _healthPreviousValue = _enemyStatus.GetHealth();
        RageSlider.value = _enemyStatus.F_RageMeter.value;
        if (_enemyStatus.B_Enraged) RageFill.color = Color.white;
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
        _enemyStatus.AffectRage(damage);
        _enemyStatus.SetHealth(_enemyStatus.GetHealth() - damage);
    }
    public void HideCanvas()
    {
        HealthCanvas.SetActive(false);
    }
    [Button("Debug Raise : Reset Health")]
    public void ResetHealth()
    {
        _enemyStatus.SetHealth(_enemyStatus.MaxHealth);
        _enemyStatus.F_RageMeter.value = 0;
        DamagedSlider.value = _enemyStatus.GetHealth();
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
        while (HealthSlider.value < _enemyStatus.MaxHealth)
        {
            time += Time.deltaTime;
            speed = _speedCurve.Evaluate(time / (_animationDuration / 2));
            HealthSlider.value = Mathf.Lerp(0f, _enemyStatus.MaxHealth, speed);
            DamagedSlider.value = Mathf.Lerp(0f, _enemyStatus.MaxHealth, speed * 4);

            yield return null;
        }

    }
}

