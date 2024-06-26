using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rain_move : MonoBehaviour
{
    [SerializeField]
    float
        fallSpeed = 0.5f,
        moveRate = 0.1f;

    private Vector3 translateVector;
    private bool fallingEnable;

    // Start is called before the first frame update
    void Start()
    {
        translateVector = new Vector3(0, fallSpeed, 0);
        fallingEnable = true;
        StartCoroutine(fall_movement());
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    private IEnumerator fall_movement()
    {
        while (fallingEnable) {
            this.transform.Translate(translateVector);
            yield return new WaitForSeconds(moveRate);
        }
    }
}
