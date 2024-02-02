using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Animation Set", menuName = "Enemy/Animation Set")]
public class SO_EnemyAnimationHandler : ScriptableObject
{
    public AnimatorController _controller;
    public void Kontol()
    {
        _controller.AddParameter("kontol", AnimatorControllerParameterType.Float);
    }
    public void Memek()
    {
        _controller.RemoveParameter(0);
    }
}
