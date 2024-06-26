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

    //弾性衝突の実装
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

            /* 流れ星生成
            ContactPoint[] contactPoints = collision.contacts;
            collisionCoordinate = contactPoints[0].point;
            Instantiate(falling_star, collisionCoordinate, Quaternion.identity);   
            */

         //UnityEngine.Debug.Log("OnCollision");
    }

    
    private  void movment_control()
    {

        // 上下の動きを加える
        float verticalMovement = Mathf.Sin(Time.time * frequencyUpDownValue) * moveUpDownFactor;
        velocity.y += verticalMovement;

        // 左右の動きを加える
        float horizontalMovement = Mathf.Sin(Time.time * frequencyLeftRightValue) * moveLeftRightFactor;
        velocity.x += horizontalMovement;

        // 最低速度調整
        if (velocity.magnitude < minVelocity)
        {
            velocity = velocity.normalized * minVelocity;
        }

        //最高速度調整
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
        /*弾性衝突
        Vector3 velocityReflect = Vector3.Reflect(velocity, collision.contacts[0].normal);
        velocity = velocityReflect;
        */

        // 衝突した面の法線を取得
        Vector3 normal = collision.contacts[0].normal;

        // 跳ね返りの角度を調整
        Vector3 velocityReflect = Vector3.Reflect(velocity, normal);
        velocityReflect = Vector3.Lerp(velocity, velocityReflect, bounceDegreeFactor);

        // ランダムな角度の偏差を加える
        float randomAngle = UnityEngine.Random.Range(-randomAngleRange, randomAngleRange);
        Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);
        velocityReflect = randomRotation * velocityReflect;

        // 衝突した地点から一定距離離れる
        transform.position += normal * minDistanceFromCollision;

        // 速度に加速度を与える
        velocity = velocityReflect * bounceAccelerationFactor;
    }

    private void spawn_flower_petals(Collision collision)
    {
        // 衝突地点と法線を取得
        ContactPoint contact = collision.contacts[0];
        Vector3 contactPoint = contact.point;
        Vector3 normal = contact.normal;

        // 円錐状にオブジェクトを生成
        for (int i = 0; i < flowerPetalSpawnNum; i++)
        {
            // ランダムな回転角度を取得
            float angle = UnityEngine.Random.Range(-flowerPetalSpreadAngle / 2.0f, flowerPetalSpreadAngle / 2.0f);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            // 法線に基づいた方向を計算
            Vector3 direction = rotation * normal;

            // 生成位置を計算
            Vector3 spawnPosition = contactPoint + direction * flowerPetalSpawnDistance;

            // オブジェクトを生成
            Instantiate(flowerPetal, spawnPosition, Quaternion.LookRotation(direction));
        }
    }

}
