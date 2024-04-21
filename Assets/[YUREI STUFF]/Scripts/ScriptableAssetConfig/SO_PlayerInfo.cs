using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Global Info",menuName ="Player/Data/Global Data")]
public class SO_PlayerInfo : ScriptableObject
{
    public Vector3 position { get; private set; }
    public Rigidbody2D rbPlayer {  get; private set; }  
    public float playerHealth {  get; private set; }


    public void StoreDynamicData(Vector3 playerPosition)
    {
        position = playerPosition;
    }
    public void StoreStaticData(Rigidbody2D rigidbody2D)
    {
        rbPlayer = rigidbody2D;
    }
}
