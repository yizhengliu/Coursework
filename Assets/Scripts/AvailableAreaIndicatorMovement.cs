using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableAreaIndicatorMovement : MonoBehaviour
{
    private float originalY;
    [SerializeField]
    private AnimationCurve loopingCurve;
    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector2(transform.position.x, 
            loopingCurve.Evaluate(Time.time) + originalY);
    }
}
