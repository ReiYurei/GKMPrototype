using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public GameObject lineProjectilePrefab;
    public LineRenderer[] lineRenderers;
    public GameObject[] lineObject;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float timeToTarget;
    public float duration;
    public bool isInstant;

    public float startWidth;
    public float endWidth;

    public float length;
    public Vector3[] target;
    public int _numberOfSegments;

    float _minAngle;
    float _maxAngle;
    public float _angleRange;
    public float _angleChange;
    public float _targetAngle;
    void OnValidate()
    {
        // Calculate the half angle range from the _target angle
        float halfAngleRange = _angleRange / 2f;

        // Adjust the min and max angles based on the _target angle and half angle range
        _minAngle = _targetAngle - halfAngleRange;
        _maxAngle = _targetAngle + halfAngleRange;

        // Calculate the angle increment for each segment

        // Ensure _minAngle is less than _maxAngle
        if (_minAngle > _maxAngle)
        {
            float temp = _minAngle;
            _minAngle = _maxAngle;
            _maxAngle = temp;
        }

    }
    private void Update()
    {
        // Calculate the half angle range from the _target angle
        float halfAngleRange = _angleRange / 2f;
        // Adjust the min and max angles based on the _target angle and half angle range
        _minAngle = _targetAngle - halfAngleRange;
        _maxAngle = _targetAngle + halfAngleRange;

        // Calculate the angle increment for each segment

        // Ensure _minAngle is less than _maxAngle
        if (_minAngle > _maxAngle)
        {
            float temp = _minAngle;
            _minAngle = _maxAngle;
            _maxAngle = temp;
        }

    }
    private void OnDrawGizmos()
    {
        DivideAngle();
    }
    void DivideAngle()
    {

        float totalAngleRange = _maxAngle - _minAngle;
        float angleIncrement = totalAngleRange / (_numberOfSegments - 1);
        for (int i = 0; i < _numberOfSegments; i++)
        {
            float angle = _minAngle + i * angleIncrement;
            if (_numberOfSegments == 1)
            {
                _targetAngle = (_minAngle + _maxAngle) / 2f;
                angle = _targetAngle;
            }
            Vector3 end = Quaternion.Euler(0, 0, angle) * Vector3.right;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (end * length));
        }
        UnityEngine.Color color = UnityEngine.Color.blue;
        Gizmos.color = color;

    }
    [Button("Initialize Line")]
    public void Test()
    {
        //lineRenderer.SetPositions(new Vector3[2] {start, length });
        lineObject = new GameObject[_numberOfSegments];
        lineRenderers = new LineRenderer[_numberOfSegments];

        for (int j = 0; j < _numberOfSegments; j++)
        {
            lineObject[j] = Instantiate(lineProjectilePrefab, transform);
            lineRenderers[j] = lineObject[j].GetComponent<LineRenderer>();
            lineRenderers[j].startWidth = startWidth;
            lineRenderers[j].endWidth = endWidth;

        }
        float totalAngleRange = _maxAngle - _minAngle;
        float angleIncrement = totalAngleRange / (_numberOfSegments - 1);

        target = new Vector3[_numberOfSegments];
        for (int j = 0; j < _numberOfSegments; j++)
        {
            float angle = _minAngle + j * angleIncrement;
            if (_numberOfSegments == 1)
            {
                _targetAngle = (_minAngle + _maxAngle) / 2f;
                angle = _targetAngle;
            }
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, length);
            if (hit.collider != null)
            {
                target[j] = direction * hit.distance;
                continue;
            }
            target[j] = direction * length;

        }

    }
    [Button("Destroy")]
    public void Destroy()
    {
        for (int j = 0; j < lineRenderers.Length; j++)
        {
            DestroyImmediate(lineRenderers[j].gameObject);
        }
        lineRenderers = null;
        lineObject = null;
        target = null;
    }
    [Button("Animate")]
    public void Animate()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateInit());
    }
    IEnumerator AnimateInit()
    {
        float time = 0f;
        float speed;
        float origin = _targetAngle;
        float targetAngleChange = _targetAngle + _angleChange;
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].SetPositions(new Vector3[2] { transform.position, transform.position });
            lineObject[i].SetActive(true);
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / timeToTarget);
            _targetAngle = Mathf.Lerp(origin, targetAngleChange, speed);

            float totalAngleRange = _maxAngle - _minAngle;
            float angleIncrement = totalAngleRange / (_numberOfSegments - 1);
            target = new Vector3[_numberOfSegments];
            for (int i = 0; i < _numberOfSegments; i++)
            {
                float angle = _minAngle + i * angleIncrement;
                if (_numberOfSegments == 1)
                {
                    _targetAngle = (_minAngle + _maxAngle) / 2f;
                    angle = _targetAngle;
                }
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, length);
                if (hit.collider != null)
                {
                    target[i] = direction * hit.distance;
                    lineRenderers[i].SetPositions(new Vector3[2] { transform.position, transform.position + target[i] });
                    continue;
                }
                target[i] = direction * length;
                lineRenderers[i].SetPositions(new Vector3[2] { transform.position, transform.position + target[i] });
            }

            yield return null;
        }
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineObject[i].SetActive(false);
        }
    }
}

