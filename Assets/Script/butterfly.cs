using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class butterfly : MonoBehaviour
{
    [SerializeField]
    float
        speedFly = 10,
        inSmallCageScale = 2f,
        inSmallCageDestroyTime = 20f,
        //speedRotate = 10,
        bounceDegreeFactor = 1.2f,
        bounceAccelerationFactor = 1.1f,
        moveUpDownFactor = 1.2f,
        moveLeftRightFactor = 1.05f,
        frequencyUpDownValue = 30f,
        frequencyLeftRightValue = 10f,
        minDistanceFromCollision = 0.5f,
        randomAngleRange = 10f,
        minVelocity = 5f,
        maxVelocity = 20f,
        flowerPetalSpreadAngle = 60.0f,
        flowerPetalSpawnDistance = 1.0f;

    [SerializeField]
    int flowerPetalSpawnNum = 5;



    Rigidbody rb;

    public GameObject flowerPetal;

    public bool inSmallCage;

    public bool phaseChangeAcceptEnable, isFirst, isCollisionDetectionEnable;

    private Vector3 collisionCoordinate;

    private phase_operator phaseOperator;

    private Stopwatch stopwatch;

    private float elapsedTime;

    private Vector3 randomOffset;


    Vector3 velocity;


    void Start()
    {
        

        if (inSmallCage)
        {
            this.transform.localScale = new Vector3(inSmallCageScale, inSmallCageScale, inSmallCageScale);   
        }

        rb = GetComponent<Rigidbody>();
        phaseOperator = FindObjectOfType<phase_operator>();
        stopwatch = new Stopwatch();
        stopwatch.Start();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        velocity = new Vector3(0.0f, speedFly, 0.0f);
        phaseChangeAcceptEnable = true;
        isFirst = true;
        isCollisionDetectionEnable = false;
    }

    private void Update()
    {
        destroyThisObject();
    }

    void FixedUpdate()
    {
        movment_control();
        rb.velocity = velocity;
    }

    //�e���Փ˂̎���
    public void OnCollisionEnter(Collision collision)
    {
        spawn_flower_petals(collision);

        if (inSmallCage)
        {
            if (collision.gameObject.CompareTag("small_cage"))
            {
                collision_moving(collision);
                UnityEngine.Debug.Log("coliision in Small cage!!");
            }
            else
            {
                Destroy(this.gameObject);
                UnityEngine.Debug.Log("Destroy!!");
            }
            UnityEngine.Debug.Log("inSmallCage!!");
        }
        else
        {
            collision_moving(collision);
        }

            /* ���ꐯ����
            ContactPoint[] contactPoints = collision.contacts;
            collisionCoordinate = contactPoints[0].point;
            Instantiate(falling_star, collisionCoordinate, Quaternion.identity);   
            */

         //UnityEngine.Debug.Log("OnCollision");
    }

    
    private  void movment_control()
    {

        // �㉺�̓�����������
        float verticalMovement = Mathf.Sin(Time.time * frequencyUpDownValue) * moveUpDownFactor;
        velocity.y += verticalMovement;

        // ���E�̓�����������
        float horizontalMovement = Mathf.Sin(Time.time * frequencyLeftRightValue) * moveLeftRightFactor;
        velocity.x += horizontalMovement;

        // �Œᑬ�x����
        if (velocity.magnitude < minVelocity)
        {
            velocity = velocity.normalized * minVelocity;
        }

        //�ō����x����
        if (velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }


        //UnityEngine.Debug.Log("Speed="+rb.velocity.magnitude);

        //velocity = Vector3.Lerp(velocity, rb.velocity, Time.deltaTime);
    }

    private void destroyThisObject()
    {
        if (inSmallCageDestroyTime < elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void collision_moving(Collision collision)
    {
        /*�e���Փ�
        Vector3 velocityReflect = Vector3.Reflect(velocity, collision.contacts[0].normal);
        velocity = velocityReflect;
        */

        // �Փ˂����ʂ̖@�����擾
        Vector3 normal = collision.contacts[0].normal;

        // ���˕Ԃ�̊p�x�𒲐�
        Vector3 velocityReflect = Vector3.Reflect(velocity, normal);
        velocityReflect = Vector3.Lerp(velocity, velocityReflect, bounceDegreeFactor);

        // �����_���Ȋp�x�̕΍���������
        float randomAngle = UnityEngine.Random.Range(-randomAngleRange, randomAngleRange);
        Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);
        velocityReflect = randomRotation * velocityReflect;

        // �Փ˂����n�_�����苗�������
        transform.position += normal * minDistanceFromCollision;

        // ���x�ɉ����x��^����
        velocity = velocityReflect * bounceAccelerationFactor;
    }

    private void spawn_flower_petals(Collision collision)
    {
        // �Փ˒n�_�Ɩ@�����擾
        ContactPoint contact = collision.contacts[0];
        Vector3 contactPoint = contact.point;
        Vector3 normal = contact.normal;

        // �~����ɃI�u�W�F�N�g�𐶐�
        for (int i = 0; i < flowerPetalSpawnNum; i++)
        {
            // �����_���ȉ�]�p�x���擾
            float angle = UnityEngine.Random.Range(-flowerPetalSpreadAngle / 2.0f, flowerPetalSpreadAngle / 2.0f);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            // �@���Ɋ�Â����������v�Z
            Vector3 direction = rotation * normal;

            // �����ʒu���v�Z
            Vector3 spawnPosition = contactPoint + direction * flowerPetalSpawnDistance;

            // �I�u�W�F�N�g�𐶐�
            Instantiate(flowerPetal, spawnPosition, Quaternion.LookRotation(direction));
        }
    }

}
