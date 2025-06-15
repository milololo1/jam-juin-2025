using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform target; //Target to follow
    [SerializeField] Vector3 offset; //Position up the enemy
    [SerializeField] Image fillImage;

    private void Start()
    {
        offset = new Vector3(0, 2, 0);
        transform.forward = Camera.main.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    public void SetHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }

    public void SetTarget (Transform currentTarget)
    {
        target = currentTarget;
    }
}
