using System;
using UnityEngine;

public class ObjectiveTimer : MonoBehaviour
{
    public event Action<float> OnUpdate;

    private void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
    }
}