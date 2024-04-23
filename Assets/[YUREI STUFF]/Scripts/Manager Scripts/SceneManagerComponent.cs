using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class SceneManagerComponent : MonoBehaviour
{
    public static SceneManagerComponent Instance { get; private set; }
    [field: Header("Event")]
    [field: SerializeField] public SO_VoidGameEvent LoadCompleteEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeOverallStateEvent { get; private set; }

    [Header("States")]
    [SerializeField] private HubState _hubState;
    [SerializeField] private ExterminateState _exterminateState;
    [SerializeField] private LoadingScreenState _loadingState;
    [SerializeField] private TitleScreenState _titleScreenState;

    [field: Header("Loading Canvas")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _player;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private Image _fade;

    private List<AsyncOperation> _loadOperation = new List<AsyncOperation>();
    private Color _fadeColor = new(0f,0f,0f,1.0f);
    private float _timeToFade = 1.5f;
    private float _fadeSpeed;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start()
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(ManagerInitialization());
    }
    private IEnumerator ManagerInitialization()
    {
        _fade.color = Color.black;
        _fade.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(Initialize());
        yield return StartCoroutine(LocalFadeOut());
        LoadCompleteEvent.Raise();
        IEnumerator Initialize()
        {
            _loadingBar.value = 0;
            float totalProgress = 0;
            _loadingScreen.SetActive(true);
            if(!SceneManager.GetSceneByName("Manager").isLoaded) _loadOperation.Add(SceneManager.LoadSceneAsync("Manager", LoadSceneMode.Additive));
            for (int i = 0; i < _loadOperation.Count; i++)
            {
                while (!_loadOperation[i].isDone)
                {
                    totalProgress += _loadOperation[i].progress;
                    _loadingBar.value = totalProgress / _loadOperation.Count;
                    yield return null;
                }
            }
            _loadingScreen.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
        IEnumerator LocalFadeOut()
        {
            _fade.color = Color.black;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != transparent)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / 2.5f);
                _fade.color = Color.Lerp(_fadeColor, transparent, _fadeSpeed);
                yield return null;
            }
            _fade.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
    }

    //Load Stage
    public void LoadStage(ScriptableObject data)
    {
        var stage = data as SO_MissionData;
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(StageInitialization(stage));
    }
    private IEnumerator StageInitialization(SO_MissionData data)
    {
        yield return StartCoroutine(LocalFadeIn());
        yield return StartCoroutine(LoadingScreen());
        yield return StartCoroutine(LocalFadeOut());
        LoadCompleteEvent.Raise();
        ChangeOverallStateEvent.Raise(_exterminateState);

        IEnumerator LocalFadeIn()
        {
            _fade.color = Color.clear;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != _fadeColor)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / _timeToFade);
                _fade.color = Color.Lerp(transparent, _fadeColor, _fadeSpeed);
                yield return null;
            }
        }
        IEnumerator LoadingScreen()
        {
            _loadingBar.value = 0;
            float totalProgress = 0;
            _loadOperation.Add(SceneManager.LoadSceneAsync(data.StageInfo.SceneName.ToString()));
            _loadOperation.Add(SceneManager.LoadSceneAsync("Manager", LoadSceneMode.Additive));
            _loadOperation.Add(SceneManager.LoadSceneAsync("UI_Dialogue", LoadSceneMode.Additive));
            _loadingScreen.SetActive(true);
            for (int i = 0; i < _loadOperation.Count; i++)
            {
                while (!_loadOperation[i].isDone)
                {
                    totalProgress += _loadOperation[i].progress;
                    _loadingBar.value = totalProgress / _loadOperation.Count;
                    yield return null;
                }
            }
            totalProgress = 0;
            var stageManager = GameObject.FindGameObjectWithTag("StageManager").GetComponent<StageManagerComponent>();
            _loadOperation.Add(InstantiateAsync(data.AstralEntity, Vector3.zero, Quaternion.identity));
            _loadOperation.Add(InstantiateAsync(_player, stageManager.playerSpawnPoint.position, Quaternion.identity));
            for (int i = 0; i < _loadOperation.Count; i++)
            {
                while (!_loadOperation[i].isDone)
                {
                    totalProgress += _loadOperation[i].progress;
                    _loadingBar.value = totalProgress / _loadOperation.Count;
                    yield return null;
                }
            }
            var astralEntity = GameObject.FindGameObjectWithTag("AstralEntity");
            astralEntity.transform.position = stageManager.astralSpawnPoint.position;
            _loadingScreen.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        IEnumerator LocalFadeOut()
        {
            _fade.color = Color.black;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != transparent)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / _timeToFade);
                _fade.color = Color.Lerp(_fadeColor, transparent, _fadeSpeed);
                yield return null;
            }
            _fade.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);

        }
    }

    //Load Title Screen
    public void LoadTitleScreen()
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(TitleScreenInitialization());

    }
    private IEnumerator TitleScreenInitialization()
    {
        yield return StartCoroutine(LocalFadeIn());
        yield return StartCoroutine(LoadingScreen());
        yield return StartCoroutine(LocalFadeOut());
        LoadCompleteEvent.Raise();

        IEnumerator LocalFadeIn()
        {
            _fade.color = Color.clear;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != _fadeColor)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / _timeToFade);
                _fade.color = Color.Lerp(transparent, _fadeColor, _fadeSpeed);
                yield return null;
            }
        }
        IEnumerator LoadingScreen()
        {
            _loadingBar.value = 0;
            float totalProgress = 0;
            _loadOperation.Add(SceneManager.LoadSceneAsync("Title_Screen"));
            if (!SceneManager.GetSceneByName("Manager").isLoaded) _loadOperation.Add(SceneManager.LoadSceneAsync("Manager", LoadSceneMode.Additive));
            _loadingScreen.SetActive(true);
            for (int i = 0; i < _loadOperation.Count; i++)
            {
                while (!_loadOperation[i].isDone)
                {
                    totalProgress += _loadOperation[i].progress;
                    _loadingBar.value = totalProgress / _loadOperation.Count;
                    yield return null;
                }
            }
            _loadingScreen.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        IEnumerator LocalFadeOut()
        {
            _fade.color = Color.black;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != transparent)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / _timeToFade);
                _fade.color = Color.Lerp(_fadeColor, transparent, _fadeSpeed);
                yield return null;
            }
            _fade.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);


        }
    }

    //Load Hub
    public void LoadHub()
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(HubInitialization());

    }
    private IEnumerator HubInitialization()
    {
        yield return StartCoroutine(LocalFadeIn());
        yield return StartCoroutine(LoadingScreen());
        yield return StartCoroutine(LocalFadeOut());
        LoadCompleteEvent.Raise();
        ChangeOverallStateEvent.Raise(_hubState);
        IEnumerator LocalFadeIn()
        {
            _fade.color = Color.clear;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != _fadeColor)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / _timeToFade);
                _fade.color = Color.Lerp(transparent, _fadeColor, _fadeSpeed);
                yield return null;
            }
        }
        IEnumerator LoadingScreen()
        {
            _loadingBar.value = 0;
            float totalProgress = 0;
            _loadOperation.Add(SceneManager.LoadSceneAsync("Hub_Scene"));
            if (!SceneManager.GetSceneByName("Manager").isLoaded) _loadOperation.Add(SceneManager.LoadSceneAsync("Manager", LoadSceneMode.Additive));
            _loadOperation.Add(SceneManager.LoadSceneAsync("UI_Listing", LoadSceneMode.Additive));
            _loadOperation.Add(SceneManager.LoadSceneAsync("UI_Shop", LoadSceneMode.Additive));
            _loadOperation.Add(SceneManager.LoadSceneAsync("UI_Dialogue", LoadSceneMode.Additive));


            _loadingScreen.SetActive(true);
            for (int i = 0; i < _loadOperation.Count; i++)
            {
                while (!_loadOperation[i].isDone)
                {
                    totalProgress += _loadOperation[i].progress;
                    _loadingBar.value = totalProgress / _loadOperation.Count;
                    yield return null;
                }
            }
            _loadingScreen.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        IEnumerator LocalFadeOut()
        {
            _fade.color = Color.black;
            var transparent = Color.clear;
            _fade.gameObject.SetActive(true);
            AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float time = 0f;
            while (_fade.color != transparent)
            {
                time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(time / _timeToFade);
                _fade.color = Color.Lerp(_fadeColor, transparent, _fadeSpeed);
                yield return null;
            }
            _fade.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
      

        }
    }

}

