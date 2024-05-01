using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class GameplayTransitionManager : MonoBehaviour
{
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent PhaseEndEvent { get; private set; }

    [SerializeField] private LoadingScreenState _noInput;
    [SerializeField] private RegularGameplayState _regularPhase;
    [SerializeField] private BulletHellGameplayState _bulletHellPhase;


    [SerializeField]private Camera _camera;
    [SerializeField] private float _defaultSize;
    [SerializeField] private Vector3 _defaultPosition;
    [SerializeField] private float _zoomOutSize;
    [SerializeField] private float _borderSpacing;

    [SerializeField] private float _timeToZoom;
    [SerializeField] private float _timeToCamTarget;
    [SerializeField] private float _timeToWallTarget;
    [SerializeField] private float _timeToEntityTarget;


    [SerializeField] private Transform _player;
    [SerializeField] private Transform _astral;

    [SerializeField] private Transform _targetCamAnchor;
    [SerializeField] private Transform testObject;

    [SerializeField] private RectTransform _wallLeft;
    [SerializeField] private RectTransform _wallRight;
    [SerializeField] private RectTransform _collissionTop;
    [SerializeField] private RectTransform _collissionBottom;

    private bool[] _isDone;
    //Regular
    private Vector3 _defaultLeftWallAnchor;
    private Vector3 _defaultRightWallAnchor;
    private Vector3 _defaultBottomWallAnchor;
    private Vector3 _defaultUpperWallAnchor;

    //Bullet Hell
    private Vector3 _bulletHellLeftWallAnchor;
    private Vector3 _bulletHellRightWallAnchor;
    private Vector3 _bulletHellBottomWallAnchor;
    private Vector3 _bulletHellUpperWallAnchor;

    [SerializeField] private AnimationCurve _easeIn = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _easeOut = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _easeInOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private int _coroutineCount;
    private StageManagerComponent _stageManager;
    [SerializeField] private bool debug;
    private GameplayState _previousState;
    private enum GameplayState
    {
        RegularPhase, BulletHellPhase
    }

    public void Start()
    {
        _camera = Camera.main;
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
        _astral = GameObject.FindGameObjectWithTag("Astral Entity")?.GetComponent<Transform>();
        _stageManager = GameObject.FindGameObjectWithTag("Stage Manager")?.GetComponent<StageManagerComponent>();
        _defaultSize = _camera.orthographicSize;
        _defaultPosition = _camera.transform.position;
        _targetCamAnchor = GameObject.FindGameObjectWithTag("Camera Anchor").GetComponent<Transform>();
        InitializeBound();
        WallInitialize();
    }
    public void InitializeBound()
    {
        _player ??= GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
        _astral ??= GameObject.FindGameObjectWithTag("Astral Entity")?.GetComponent<Transform>();
        var targetPos = _targetCamAnchor.position;
        var defaultPos = _defaultPosition;
        var camAspect = _camera.aspect;

        _defaultLeftWallAnchor  = defaultPos + new Vector3(-camAspect * _defaultSize * _borderSpacing * 2, _defaultSize);
        _defaultRightWallAnchor  = defaultPos + new Vector3(camAspect * _defaultSize * _borderSpacing * 2, _defaultSize);
        _defaultBottomWallAnchor  = defaultPos + new Vector3(-camAspect * _defaultSize * _borderSpacing * 2, -_defaultSize);
        _defaultUpperWallAnchor  = defaultPos + new Vector3(camAspect * _defaultSize * _borderSpacing * 2, _defaultSize);

        _bulletHellLeftWallAnchor  = targetPos + new Vector3(-camAspect * _zoomOutSize / _borderSpacing , _zoomOutSize);
        _bulletHellRightWallAnchor  = targetPos + new Vector3(camAspect * _zoomOutSize / _borderSpacing, _zoomOutSize);
        _bulletHellBottomWallAnchor  = targetPos + new Vector3(-camAspect * _zoomOutSize , -_zoomOutSize);
        _bulletHellUpperWallAnchor = targetPos + new Vector3(camAspect * _zoomOutSize , _zoomOutSize);
    }
    [Button("Bound")]
    public void Bound()
    {
        var targetPos = _targetCamAnchor.position;
        var defaultPos = _defaultPosition;
        var camAspect = _camera.aspect;
        //dont divide by 2 for accurate wallCorner, divide it by 2 means you cut the size half inward
        var defaultUpperLeft = defaultPos + new Vector3(-camAspect * _defaultSize * 2, _defaultSize); 
        var defaultUpperRight = defaultPos + new Vector3(camAspect * _defaultSize * 2, _defaultSize);
        var defaultLowerLeft = defaultPos + new Vector3(-camAspect * _defaultSize * 2, -_defaultSize);
        var defaultLowerRight = defaultPos + new Vector3(camAspect * _defaultSize * 2, -_defaultSize);

        var targetUpperLeft = targetPos + new Vector3(-camAspect * _zoomOutSize / 2, _zoomOutSize);
        var targetUpperRight = targetPos + new Vector3(camAspect * _zoomOutSize / 2, _zoomOutSize);
        var targetLowerLeft = targetPos + new Vector3(-camAspect * _zoomOutSize / 2, -_zoomOutSize);
        var targetLowerRight = targetPos + new Vector3(camAspect * _zoomOutSize / 2, -_zoomOutSize);

        //Dynamic Cam Size upper Corner
        //testObject.transform.position = _camera.transform.position + new Vector3(-camAspect * (_camera.orthographicSize /2), _camera.orthographicSize);
        Debug.Log(defaultUpperLeft);

    }
    [Button("Bound2")]
    public void Bound2()
    {
        //wallWidth of the vector = vector length
        Vector3[] v = new Vector3[4];

        _wallLeft.GetWorldCorners(v);
        var leftCorner = v[0];
        var rightCorner = v[3];
        float width = Vector3.Distance(leftCorner, rightCorner);
        Debug.Log(leftCorner + " , " + rightCorner);
        Debug.Log(width);
        Vector3 targetPos = new Vector3(_targetCamAnchor.position.x - (width/2), _wallLeft.position.y, _wallLeft.position.z);
        _wallLeft.position = targetPos;


    }
    [Button("Debug Raise : Bullet Hell phase")]
    public void BulletHellPhase()
    {
        StopAllCoroutines();
        InitializeBound();   
        _coroutineCount = 4;
        _isDone = new bool[_coroutineCount];
        for (int i = 0; i < _coroutineCount; i++) _isDone[i] = false;
        StartCoroutine(CameraZoom(GameplayState.BulletHellPhase));
        StartCoroutine(CameraMove(GameplayState.BulletHellPhase));
        StartCoroutine(Wall(GameplayState.BulletHellPhase));
        StartCoroutine(MoveEntitiesToPoint(GameplayState.BulletHellPhase));

        StartCoroutine(CoroutineProgress(GameplayState.BulletHellPhase));
        _previousState = GameplayState.BulletHellPhase;

    }
    [Button("Debug Raise : Regular phase")]
    public void RegularPhase()
    {
        StopAllCoroutines();
        InitializeBound();
        _coroutineCount = 4;
        _isDone = new bool[_coroutineCount];
        for(int i = 0; i < _coroutineCount; i++)  _isDone[i] = false;
        StartCoroutine(CameraZoom(GameplayState.RegularPhase));
        StartCoroutine(CameraMove(GameplayState.RegularPhase));
        StartCoroutine(Wall(GameplayState.RegularPhase));
        StartCoroutine(MoveEntitiesToPoint(GameplayState.RegularPhase));

        StartCoroutine(CoroutineProgress(GameplayState.RegularPhase));
        _previousState = GameplayState.RegularPhase;

    }
    IEnumerator CoroutineProgress(GameplayState phase)
    {

        for (int i = 0; i < _isDone.Length; i++)
        {
            Debug.Log("Done :" + i);
            while (!_isDone[i])
            {
                yield return null;
            }
        }
        switch (phase)
        {
            case GameplayState.RegularPhase:
                ChangeStateEvent.Raise(_regularPhase);
                break;


            case GameplayState.BulletHellPhase:
                ChangeStateEvent.Raise(_bulletHellPhase);
                break;
        }
 
        PhaseEndEvent.Raise();
        Debug.Log("Phase Done");

    }
    public void WallInitialize()
    {
        Vector3[] wallCorner = new Vector3[4];
        Vector3[] collissionCorner = new Vector3[4];
        _wallLeft.GetWorldCorners(wallCorner);
        _collissionTop.GetWorldCorners(collissionCorner);
        var leftCorner = wallCorner[0];
        var rightCorner = wallCorner[3];
        var upperCorner = collissionCorner[0];
        var bottomCorner = collissionCorner[1];
        float wallWidth = Vector3.Distance(leftCorner, rightCorner);
        float collissionHeight = Vector3.Distance(upperCorner, bottomCorner);
        Vector3 targetPosLeft = new Vector3(_defaultLeftWallAnchor.x - (wallWidth / 2), _wallLeft.position.y, _wallLeft.position.z);
        Vector3 targetPosRight = new Vector3(_defaultRightWallAnchor.x + (wallWidth / 2), _wallRight.position.y, _wallRight.position.z);
        Vector3 targetPosUp = new Vector3(_collissionTop.position.x, _defaultUpperWallAnchor.y + (collissionHeight / 2));
        Vector3 targetPosBottom = new Vector3(_collissionBottom.position.x, _defaultBottomWallAnchor.y - (collissionHeight / 2));


        _wallLeft.transform.position = targetPosLeft;
        _wallRight.transform.position = targetPosRight;
        _collissionTop.position = targetPosUp;
        _collissionBottom.position = targetPosBottom;
    }
    IEnumerator Wall(GameplayState phase)
    {
        float time = 0f;
        float speed;
        Vector3[] wallCorner = new Vector3[4];
        Vector3[] collissionCorner = new Vector3[4];
        _wallLeft.GetWorldCorners(wallCorner);
        _collissionTop.GetWorldCorners(collissionCorner);
        var leftCorner = wallCorner[0];
        var rightCorner = wallCorner[3];
        var upperCorner = collissionCorner[0];
        var bottomCorner = collissionCorner[1];
        float wallWidth = Vector3.Distance(leftCorner, rightCorner);
        float collissionHeight = Vector3.Distance(upperCorner, bottomCorner);
        var defaultPos = _defaultPosition;
        var camAspect = _camera.aspect;
        Vector3 initialLeftPos;
        Vector3 InitialRightPos;
        Vector3 initialUpPos;
        Vector3 InitialBottomPos;

        Vector3 targetPosLeft; 
        Vector3 targetPosRight;
        Vector3 targetPosUp;
        Vector3 targetPosBottom;
        switch (phase)
        {
            case GameplayState.RegularPhase:
                initialLeftPos = new Vector3(_bulletHellLeftWallAnchor.x - (wallWidth / 2), _wallLeft.position.y, _wallLeft.position.z);
                InitialRightPos = new Vector3(_bulletHellRightWallAnchor.x + (wallWidth / 2), _wallRight.position.y, _wallRight.position.z);
                initialUpPos = new Vector3(_collissionTop.position.x, _defaultUpperWallAnchor.y + (collissionHeight / 2));
                InitialBottomPos = new Vector3(_collissionBottom.position.x, _defaultBottomWallAnchor.y - (collissionHeight / 2));

                Vector3 finalPosLeftLocation = defaultPos + new Vector3(-camAspect * _defaultSize , _defaultSize);
                Vector3 finalPosRightLocation = defaultPos + new Vector3(camAspect * _defaultSize , _defaultSize);
                targetPosLeft = new Vector3(finalPosLeftLocation.x - (wallWidth / 2), _wallLeft.position.y, _wallLeft.position.z);
                targetPosRight = new Vector3(finalPosRightLocation.x + (wallWidth / 2), _wallLeft.position.y, _wallLeft.position.z);

                targetPosUp = new Vector3(_collissionTop.position.x, _defaultUpperWallAnchor.y + (collissionHeight / 2));
                targetPosBottom = new Vector3(_collissionBottom.position.x, _defaultBottomWallAnchor.y - (collissionHeight / 2));

                if (debug)
                {
                    _wallLeft.transform.position = targetPosLeft;
                    _wallRight.transform.position = targetPosBottom;

                    _collissionTop.position = targetPosUp;
                    _collissionBottom.position = targetPosBottom;

                    break;
                }
                if (_previousState == GameplayState.RegularPhase) break;
                _collissionTop.position = initialUpPos * 4;
                _collissionBottom.position = InitialBottomPos * 4;

                if (_wallLeft.transform.position == targetPosLeft &
                    _wallRight.transform.position == targetPosRight) 
                {
                    _collissionTop.position = targetPosUp;
                    _collissionBottom.position = targetPosBottom;
                    break;
                }
                while (time < _timeToWallTarget)
                {
                    time += Time.deltaTime;
                    speed = _easeIn.Evaluate(time / _timeToWallTarget);
                    _wallLeft.transform.position = Vector3.Lerp(initialLeftPos, targetPosLeft * 2, speed);
                    _wallRight.transform.position = Vector3.Lerp(InitialRightPos, targetPosRight * 2, speed);
                    yield return null;
                }
                _wallLeft.transform.position = targetPosLeft;
                _wallRight.transform.position = targetPosRight;

                _collissionTop.position = targetPosUp;
                _collissionBottom.position = targetPosBottom;
                break;

            case GameplayState.BulletHellPhase:

                initialLeftPos = new Vector3(_defaultLeftWallAnchor.x - (wallWidth / 2), _wallLeft.position.y, _wallLeft.position.z);
                InitialRightPos = new Vector3(_defaultRightWallAnchor.x + (wallWidth / 2), _wallRight.position.y, _wallRight.position.z);
                initialUpPos = new Vector3(_collissionTop.position.x, _defaultUpperWallAnchor.y + (collissionHeight / 2));
                InitialBottomPos = new Vector3(_collissionBottom.position.x, _defaultBottomWallAnchor.y - (collissionHeight / 2));

                targetPosLeft = new Vector3(_bulletHellLeftWallAnchor.x - (wallWidth / 2), _wallLeft.position.y, _wallLeft.position.z);
                targetPosRight = new Vector3(_bulletHellRightWallAnchor.x + (wallWidth / 2), _wallRight.position.y, _wallRight.position.z);
                targetPosUp = new Vector3(_collissionTop.position.x, _bulletHellUpperWallAnchor.y + (collissionHeight / 2));
                targetPosBottom = new Vector3(_collissionBottom.position.x, _bulletHellBottomWallAnchor.y - (collissionHeight / 2));

                if (debug)
                {
                    _wallLeft.transform.position = targetPosLeft;
                    _wallRight.transform.position = targetPosRight;

                    _collissionTop.position = targetPosUp;
                    _collissionBottom.position = targetPosBottom;
                    break;
                }
                if (_previousState == GameplayState.BulletHellPhase) break;
                _collissionTop.position = initialUpPos * 4;
                _collissionBottom.position = InitialBottomPos * 4;

                if (_wallLeft.transform.position == targetPosLeft &
                    _wallRight.transform.position == targetPosRight) 
                {
                    _collissionTop.position = targetPosUp;
                    _collissionBottom.position = targetPosBottom;
                    break;
                }
                while (time < _timeToWallTarget)
                {
                    time += Time.deltaTime;
                    speed = _easeIn.Evaluate(time / _timeToWallTarget);
                    _wallLeft.transform.position = Vector3.Lerp(initialLeftPos, targetPosLeft, speed);
                    _wallRight.transform.position = Vector3.Lerp(InitialRightPos, targetPosRight, speed);
                    yield return null;
                }
                _wallLeft.transform.position = targetPosLeft;
                _wallRight.transform.position = targetPosRight;
                _collissionTop.position = targetPosUp;
                _collissionBottom.position = targetPosBottom;
                break;
        }
        _isDone[2] = true;
        yield break;
    }
    IEnumerator CameraZoom(GameplayState phase)
    {
        float time = 0f;
        float speed;
        switch (phase)
        {
            case GameplayState.RegularPhase:
                if (debug)
                {
                    _camera.orthographicSize = _defaultSize;
                    break;
                }
                if (_previousState == GameplayState.RegularPhase) break;
                if (_camera.orthographicSize == _defaultSize) break;
                while (_camera.orthographicSize > _defaultSize)
                {
                    time += Time.deltaTime;
                    speed = _easeIn.Evaluate(time / _timeToZoom);
                    _camera.orthographicSize = Mathf.Lerp(_zoomOutSize, _defaultSize, speed);
                    yield return null;
                }
                _camera.orthographicSize = _defaultSize;
                break;

            case GameplayState.BulletHellPhase:
                if (debug)
                {
                    _camera.orthographicSize = _zoomOutSize;
                    break;
                }
                if (_previousState == GameplayState.BulletHellPhase) break;
                if (_camera.orthographicSize == _zoomOutSize) break;

                while (_camera.orthographicSize < _zoomOutSize)
                {
                    time += Time.deltaTime;
                    speed = _easeInOut.Evaluate(time / _timeToZoom);
                    _camera.orthographicSize = Mathf.Lerp(_defaultSize, _zoomOutSize, speed);
                    yield return null;
                }
                _camera.orthographicSize = _zoomOutSize;
                break;
        }
        _isDone[1] = true;

        yield break;
    }
    IEnumerator CameraMove(GameplayState phase)
    {
        float time = 0f;
        float speed;
        Vector3 anchorPos = new Vector3(_targetCamAnchor.position.x, _targetCamAnchor.position.y, _defaultPosition.z);

        switch (phase)
        {
            case GameplayState.RegularPhase:
                if (debug)
                {
                    _camera.transform.position = _defaultPosition;
                    break;
                }
                if (_previousState == GameplayState.RegularPhase) break;
                if (_camera.transform.position == _defaultPosition) break;
                while (_camera.transform.position != _defaultPosition)
                {
                    time += Time.deltaTime;
                    speed = _easeIn.Evaluate(time / _timeToCamTarget);
                    _camera.transform.position = Vector3.Lerp(anchorPos, _defaultPosition, speed);
                    yield return null;
                }
                _camera.transform.position = _defaultPosition;
                break;
            case GameplayState.BulletHellPhase:
                if (debug)
                {
                    _camera.transform.position = anchorPos;
                    break;
                }
                if (_previousState == GameplayState.BulletHellPhase) break;
                if (_camera.transform.position == anchorPos) break;
                while (_camera.transform.position != anchorPos)
                {
                    time += Time.deltaTime;
                    speed = _easeInOut.Evaluate(time / _timeToCamTarget);
                    _camera.transform.position = Vector3.Lerp(_defaultPosition, anchorPos, speed);
                    yield return null;
                }
                _camera.transform.position = anchorPos;
                break;
         
        }
        _isDone[0] = true;

        yield break;
    }
    IEnumerator MoveEntitiesToPoint(GameplayState phase)
    {
        if (_stageManager == null || _player == null|| _astral == null ||!_stageManager.SpawnPointReady())
        {
            _isDone[3] = true;
            Debug.Log("<color=yellow>Player or Entity is not properly initialized!</color>");
            yield break;
        }
        float time = 0;
        float speed;
        Vector3 originPlayerPos= _player.position; 
        Vector3 originAstralPos= _astral.position;

        Vector3 targetPlayerPos;
        Vector3 targetAstralPos;
        Vector3 arcPoint1;
        Vector3 arcPoint2;
        float offset1;
        float offset2;
        switch (phase)
        {
            case GameplayState.RegularPhase:
                targetPlayerPos = _stageManager.playerSpawnPoint.position;
                targetAstralPos = _stageManager.astralSpawnPoint.position;
                offset1 = Vector3.Distance(_stageManager.playerSpawnPoint.position,_player.position);
                offset2 = Vector3.Distance(_stageManager.astralSpawnPoint.position, _astral.position);
                arcPoint1 = new Vector3(Random.Range(-10, 0), offset1 / 2, _player.position.z);
                arcPoint2 = new Vector3(Random.Range(0, 10), offset2 / 2, _astral.position.z);
                if (_previousState == GameplayState.RegularPhase) break;
                if (_player.position == targetPlayerPos) break;
                while (time < _timeToEntityTarget)
                {
                    time += Time.deltaTime;
                    speed = _easeInOut.Evaluate(time / _timeToEntityTarget);
                    //Player Move to Point
                    Vector3 ab1 = Vector3.Lerp(originPlayerPos, arcPoint1, speed);
                    Vector3 bc1 = Vector3.Lerp(arcPoint1, targetPlayerPos, speed);
                    _player.position = Vector3.Lerp(ab1, bc1, speed);
                    //Astral Move to Point
                    Vector3 ab2 = Vector3.Lerp(originAstralPos, arcPoint2, speed);
                    Vector3 bc2 = Vector3.Lerp(arcPoint2, targetAstralPos, speed);
                    _astral.position = Vector3.Lerp(ab2, bc2, speed);

                    yield return null;
                }

                break;
            case GameplayState.BulletHellPhase:
                targetPlayerPos = _stageManager.playerBulletHellPoint.position;
                targetAstralPos = _stageManager.astralBulletHellPoint.position;
                offset1 = Vector3.Distance(_stageManager.playerBulletHellPoint.position, _player.position);
                offset2 = Vector3.Distance(_stageManager.playerBulletHellPoint.position, _astral.position);
                arcPoint1 = new Vector3(Random.Range(-10, 0), offset1 / 2, _player.position.z);
                arcPoint2 = new Vector3(Random.Range(0, 10), offset2 / 2, _astral.position.z);
                if (_previousState == GameplayState.BulletHellPhase) break;
                if (_player.position == targetPlayerPos) break;
                while (time < _timeToEntityTarget)
                {
                    time += Time.deltaTime;
                    speed = _easeInOut.Evaluate(time / _timeToEntityTarget);
                    //Player Move to Point
                    Vector3 ab1 = Vector3.Lerp(originPlayerPos, arcPoint1, speed);
                    Vector3 bc1 = Vector3.Lerp(arcPoint1, targetPlayerPos, speed);
                    _player.position = Vector3.Lerp(ab1, bc1, speed);

                    //Astral Move to Point
                    Vector3 ab2 = Vector3.Lerp(originAstralPos, arcPoint2, speed);
                    Vector3 bc2 = Vector3.Lerp(arcPoint2, targetAstralPos, speed);
                    _astral.position = Vector3.Lerp(ab2, bc2, speed);
                    yield return null;
                }
                break;

        }
        _isDone[3] = true;

        yield break;
    }

}

