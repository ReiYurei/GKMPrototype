using System.Collections;
using UnityEngine;

public class AfterimageVFX : MonoBehaviour
{
    public SpriteRenderer _renderer;
    public SpriteRenderer _spine;

    public float interval;
    public float initializationTime;
    public float disperseTime;

    private float _interval;
    public float catchUpSpeed;
    public Color color;
    private Vector3 _velocity = Vector3.zero;
    private void Start()
    {
         _interval = interval;
        _renderer.material.SetColor("Color", color);
        _renderer.color = Color.clear;
    }
    private void FixedUpdate()
    {
        _interval -= Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, _spine.gameObject.transform.position, ref _velocity , catchUpSpeed * Time.fixedDeltaTime);
        if (_interval < 0)
        {
            _interval = interval;
            _renderer.sprite = _spine.sprite;
            transform.rotation = _spine.transform.rotation;
        }

    }
    public void AfterImageStart()
    {
        StartCoroutine(Initialize());
        IEnumerator Initialize()
        {
            _renderer.color = Color.clear;
            float time = 0f;
            float speed;
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            while(time < initializationTime)
            {
                time += Time.deltaTime;
                speed = curve.Evaluate(time / initializationTime);
                _renderer.color = Color.Lerp(Color.clear, color, speed);
                yield return null;
            }
        }
    }
    public void AfterImageStop()
    {
        StartCoroutine(Initialize());
        IEnumerator Initialize()
        {
            float time = 0f;
            float speed;
            gameObject.SetActive(true);
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            while (time < disperseTime)
            {
                time += Time.deltaTime;
                speed = curve.Evaluate(time / disperseTime);
                _renderer.color = Color.Lerp(color, Color.clear, speed);
                yield return null;
            }
            gameObject.SetActive(false);

        }
    }
}
