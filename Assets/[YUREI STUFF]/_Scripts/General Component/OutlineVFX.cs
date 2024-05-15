using UnityEngine;
using System.Collections;
using TriInspector;
public class OutlineVFX : MonoBehaviour, IAudioSource
{
    [field: SerializeField]public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    public SpriteRenderer _parentRenderer;
    public bool interactableOutline;
    [ShowIf(nameof(interactableOutline),true)]public BoxCollider2D _parentCollider;
    public float lineGrowTime;
    public float lineThickness;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _renderer;


    private void Start()
    {
        AudioCollection.InitializeStartData();
        _renderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        if (interactableOutline)
        {
            _boxCollider.size = _parentCollider.size;
            _boxCollider.offset = _parentCollider.offset;
        }
    }
    private void Update()
    {
        _renderer.sprite = _parentRenderer.sprite;
    }

    IEnumerator ShowOutline()
    {
        float time = 0f;
        float speed;
        float value;
        AudioCollection.Play_OneShot("Selectable");
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while (time < lineGrowTime)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / lineGrowTime);
            value = Mathf.Lerp(0, lineThickness, speed);
            _renderer.material.SetFloat("_LineThickness", value);
            yield return null;
        }
        yield break;
    }
    IEnumerator HideOutline()
    {
        float time = 0f;
        float speed;
        float value;
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while (time < lineGrowTime)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / lineGrowTime);
            value = Mathf.Lerp(lineThickness, 0, speed);
            _renderer.material.SetFloat("_LineThickness", value);

            yield return null;
        }
        yield break;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactableOutline) return;
        if (!collision.CompareTag("Player")) return;
        Debug.Log("Collided with : " + collision.name);
        StartCoroutine(ShowOutline());
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit  : " + collision.name);

        if (!interactableOutline) return;
        if (!collision.CompareTag("Player")) return;
        Debug.Log("Collided with : " + collision.name);
        StartCoroutine(HideOutline());
        
    }
}