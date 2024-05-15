using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    public static CameraBorder Instance { get; private set; }
    //Regular
    [SerializeField]private float _margin;
    public Vector3 defaultLeftWallAnchor;
    public Vector3 defaultRightWallAnchor;
    public Vector3 defaultBottomWallAnchor;
    public Vector3 defaultUpperWallAnchor;
    public bool debug;
    public void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
        //else
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(this);
        //}
        Instance = this;

    }
    private void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        var _camera = Camera.main;
        var _defaultSize = _camera.orthographicSize;
        var _defaultPosition = _camera.transform.position;
        var camAspect = _camera.aspect;

        defaultLeftWallAnchor = _defaultPosition + new Vector3(-camAspect * _defaultSize * _margin , _defaultSize);
        defaultRightWallAnchor = _defaultPosition + new Vector3(camAspect * _defaultSize * _margin , _defaultSize);
        defaultBottomWallAnchor = _defaultPosition + new Vector3(camAspect * _defaultSize  , -_defaultSize * _margin);
        defaultUpperWallAnchor = _defaultPosition + new Vector3(camAspect * _defaultSize, _defaultSize * _margin);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (!debug) return;
        Gizmos.DrawLine(defaultLeftWallAnchor, defaultRightWallAnchor);
        Gizmos.DrawLine(defaultBottomWallAnchor, defaultUpperWallAnchor );
    }
}