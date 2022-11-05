using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 篝火管理器
/// </summary>
public class Campfire_Conllider : MonoBehaviour
{
    [SerializeField] new Light light;
    private float time = 20;        // 最大的燃烧时间
    private float currentTime = 20; // 当前剩余的燃烧时间

    private void Update()
    {
        if (currentTime <= 0)
        {
            currentTime = 0;
            light.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            currentTime -= Time.deltaTime;
            light.intensity = Mathf.Clamp(currentTime / time, 0, 1) * 10f;
        }
    }
    public void AddWood()
    {
        currentTime += 20;
        light.transform.parent.gameObject.SetActive(true);
    }
}
