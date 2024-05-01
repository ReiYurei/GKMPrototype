using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

public class LineController : MonoBehaviour 
{
    public LineRenderer lineRenderer;
    public Vector3 start;
    public float timeToTarget;
    public Vector3 length;
    public Vector3 target;

    [Button("Spawn Line")]
    public void Test()
    {
        lineRenderer.SetPositions(new Vector3[2] {start, length });
    }
    [Button("Animate")]
    public void Animate()
    {
        StartCoroutine(AnimateInit());
    }
    IEnumerator AnimateInit()
    {
        float time = 0f;
        float speed;
        Vector3 targetPos;
        lineRenderer.SetPosition(0, start);
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        while(lineRenderer.GetPosition(1)  != target)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time /timeToTarget);
            targetPos = Vector3.Lerp(lineRenderer.GetPosition(1), target, speed);
            lineRenderer.SetPosition(1, targetPos);
            yield return null;
        }

    }
}

