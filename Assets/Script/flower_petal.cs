using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class flower_petal : MonoBehaviour
{
    [SerializeField]
    float
        destroyTime = 5.0f,
        gravity = 4.9f,
        rotationSpeed = 60.0f;


    private Vector3 rotationAxis;

    private Rigidbody rb;

    public Vector3 customGravity; // �J�X�^���d�͉����x

    private Stopwatch stopwatch;

    private float elapsedTime;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        customGravity =  new Vector3(0, gravity, 0);

        stopwatch = new Stopwatch();
        stopwatch.Start();

        // �����_���ȉ�]����ݒ�
        rotationAxis = new Vector3(Random.value, Random.value, Random.value).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        // �I�u�W�F�N�g�������_���Ȏ��ŉ�]
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        rb.AddForce(customGravity, ForceMode.Acceleration);

        elapsedTime = stopwatch.ElapsedMilliseconds / 1000f;
        destroyThisObject();
    }

    private void destroyThisObject()
    {
        if (destroyTime < elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

}
