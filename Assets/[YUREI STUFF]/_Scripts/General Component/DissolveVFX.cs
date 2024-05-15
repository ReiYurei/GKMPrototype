using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class DissolveVFX : MonoBehaviour
{
    [field: SerializeField] public Material DissolveMaterial { get; private set; }
    [SerializeField]private SpriteRenderer _renderer;
    private int _dissolve = Shader.PropertyToID("_Dissolve");
    private int _outline = Shader.PropertyToID("_Outline");
    private int _noise = Shader.PropertyToID("_Noise");
    private int _uv = Shader.PropertyToID("_UV");
    private int _color = Shader.PropertyToID("_Color");

    public Color color;
    [Range(0,1)]public float dissolveAmount;
    [Range(0, 10f)] public float outlineAmount;
    public bool animateNoise;
    public float noiseAmount;
    [ShowIf(nameof(animateNoise), true)]public float targetNoise;
    public bool animateUV;
    public Vector2 uv;
    [ShowIf(nameof(animateUV), true)] public Vector2 targetUV;
    public float timeToDissolve;
    public void StartDissolve()
    {
        _renderer = GetComponentInParent<SpriteRenderer>();
        _renderer.material = DissolveMaterial;
        StartCoroutine(InitializeDissolve());
    }
    IEnumerator InitializeDissolve()
    {
        StartCoroutine(Dissolve());
        StartCoroutine(Noise());
        StartCoroutine(UV());
        yield return new WaitForSeconds(timeToDissolve);

    }
    IEnumerator Dissolve()
    {
        float time = 0f;
        float speed;
        _renderer.material.SetFloat(_outline, outlineAmount);
        _renderer.material.SetColor(_color, color);
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while(time < timeToDissolve)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / timeToDissolve);
            dissolveAmount = Mathf.Lerp(0f, 1f, speed);
            _renderer.material.SetFloat(_dissolve, dissolveAmount);
            yield return null;
        }
    }
    IEnumerator Noise()
    {
        if (!animateNoise)
        {
            _renderer.material.SetFloat(_noise, noiseAmount);
            yield break;
        }
        float time = 0f;
        float speed;
        float noise;
        _renderer.material.SetFloat(_outline, outlineAmount);
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while (time < timeToDissolve)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / timeToDissolve);
            noise = Mathf.Lerp(noiseAmount, targetNoise, speed);
            _renderer.material.SetFloat(_noise, noise);
            yield return null;
        }
    }
    IEnumerator UV()
    {
        if (!animateUV)
        {
            _renderer.material.SetVector(_uv, uv);
            yield break;
        }
        float time = 0f;
        float speed;
        Vector2 animatedUV;
        _renderer.material.SetFloat(_outline, outlineAmount);
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while (time < timeToDissolve)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / timeToDissolve);
            animatedUV = Vector2.Lerp(uv, targetUV, speed);
            _renderer.material.SetVector(_uv, animatedUV);
            yield return null;
        }
    }
}

