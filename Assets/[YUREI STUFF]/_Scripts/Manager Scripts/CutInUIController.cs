using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class CutInUIController : MonoBehaviour,IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public GameObject CutInCanvas { get; private set; }
    [field: SerializeField] public Image CutInImage { get; private set; }
    [field: SerializeField] public RectTransform CutInMask {  get; private set; }
    [field: SerializeField] public RectTransform LeftBorder { get; private set; }
    [field: SerializeField] public RectTransform RightBorder { get; private set; }

    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent CutInEnd { get; private set; }

    [Header("Cut In Properties")]
    public AnimationCurve cutInEaseIn = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve cutInEaseOut = AnimationCurve.Linear(0, 0, 1, 1);

    public float cutInDuration;
    public float targetWidth;
    [Header("State")]
    [SerializeField] private LoadingScreenState _loadingState;
    private void Start()
    {
        AudioCollection.InitializeStartData();
    }
    public void OnLoadComplete()
    {
        CutInImage.sprite = GameObject.FindGameObjectWithTag("Astral Entity")?.GetComponent<Enemy>().StatusData.EnemyCutInSprite;
    }

    [Button("Debug Play Cut In")]
    public void OnBulletHellPhase()
    {

        ChangeStateEvent.Raise(_loadingState);
        StopAllCoroutines();
        StartCoroutine(CutInAnimation());
        StartCoroutine(Move());

    }
    IEnumerator Move()
    {
        AnimationCurve linear = AnimationCurve.Linear(0, 0, 1, 1);
        CutInMask.anchoredPosition = new Vector2(0, 0);
        CutInMask.anchoredPosition = new Vector2(0, CutInMask.anchoredPosition.y - 40);
        float originPos = CutInMask.anchoredPosition.y;
        float time = 0f;
        float speed;
        float pos;

        while (CutInMask.anchoredPosition.y < 0f)
        {
            time += Time.deltaTime;
            speed = linear.Evaluate(time / 8f);
            pos = Mathf.Lerp(originPos, 0f, speed);
            CutInMask.anchoredPosition = new Vector2(0, pos);
            yield return null;

        }
    }
    IEnumerator CutInAnimation()
    {
        CutInCanvas.SetActive(true);
        CutInMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        float time = 0f;
        float speed;
        float width;
        AudioCollection.Play_OneShot("Cut In");
        while (CutInMask.rect.width < targetWidth)
        {
            time += Time.deltaTime;
            speed = cutInEaseIn.Evaluate(time / (cutInDuration / 2));
            width = Mathf.Lerp(1f, targetWidth, speed);
            CutInMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            LeftBorder.anchoredPosition = new Vector2(CutInMask.rect.xMin, CutInMask.anchorMin.y);
            RightBorder.anchoredPosition = new Vector2(CutInMask.rect.xMax, CutInMask.anchorMin.y);
            yield return null;
        }
        time = 0f;
        while (CutInMask.rect.width > 0)
        {
            time += Time.deltaTime;
            speed = cutInEaseOut.Evaluate(time / (cutInDuration / 2));
            width = Mathf.Lerp(targetWidth, 0, speed);
            CutInMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            LeftBorder.anchoredPosition = new Vector2(CutInMask.rect.xMin, CutInMask.anchorMin.y);
            RightBorder.anchoredPosition = new Vector2(CutInMask.rect.xMax, CutInMask.anchorMin.y);
            yield return null;
        }
        StopCoroutine(Move());
        CutInCanvas.SetActive(false);
        CutInEnd.Raise();
    }
}
