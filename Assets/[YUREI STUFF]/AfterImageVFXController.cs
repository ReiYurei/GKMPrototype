using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(EventListenerComponent))]
public class AfterImageVFXController : MonoBehaviour
{
    public List<AfterimageVFX> afterImages;
    private void Start()
    {
        afterImages ??= new List<AfterimageVFX>();
        var child = GetComponentsInChildren<AfterimageVFX>();
        for (int i = 0; i < child.Length; i++)
        {
            afterImages.Add(child[i]);
        }
    }
    public void StartAfterImage()
    {
        foreach (var afterImage in afterImages)
        {
            afterImage.gameObject.SetActive(true);
            afterImage.AfterImageStart();
        }
    }
    public void StopAfterImage()
    {
        foreach (var afterImage in afterImages)
        {
            afterImage.AfterImageStop();
        }
    }
}
