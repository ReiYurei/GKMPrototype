using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Substates_Scanning", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Scanning")]
public class Scanning : MonoBehaviour
{

    public GameObject _player;
    public Rigidbody2D _playerRb;
    public Transform _playerLocation;
    public float _distance;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        _playerLocation = _player.transform;

    }
    void LateUpdate()
    {
        DEBUG_MESSAGE = $"IsPlayerJump = {CheckPlayerJump()}, Distance = {GetDistance()}, Player Location = {GetPlayerLocation()}";
    }
    public float GetDistance()
    {
        return Vector2.Distance(this.transform.position, _playerLocation.position);
    }

    public bool CheckPlayerJump()
    {
        if (_playerRb.velocity.y > 0.1f && _playerRb != null)
        {
            return true;
        }
        else if (_playerLocation.position.y > this.transform.position.y)
        {
            return true;
        }
        return false;
    }

    [SerializeField, TextArea(4, 10)]
    string DEBUG_MESSAGE;

    
    public Vector2 GetPlayerLocation()
    {
        return _playerLocation.position;

    }
    // [SerializeField] private string _name = "Enemy_Scan";
    // public override void Execute()
    // {
    //     Debug.Log("Scanning");
    // }
    // public override string GetName()
    // {
    //     return _name;
    // }
}

