using System;
using UnityEngine;

public class RecyleClickEffectAfterTime : MonoBehaviour
{
    public float time = 10.0f;
    private float leftTime = 1;
    private void Start()
    {
        this.leftTime = time;
    }
    private void Update()
    {
        this.leftTime -= Time.deltaTime;
        if (this.leftTime <= 0f)
        {
            this.leftTime = time;
            gameObject.SetActive(false);
            MapEditor.MapObjectRoot.ins.RecyleClickEffect(gameObject);
        }
    }
}
