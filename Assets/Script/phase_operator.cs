using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phase_operator : MonoBehaviour
{
    public int phase;
    public GameObject butterfly;
    public GameObject cage;
    public GameObject particle;
    public GameObject flowerPetal;
    [SerializeField]
    float
        phaseCycle = 30,
        butterflySpawnCircleSizeFirst = 3,
        butterflySpawnDistanceTime = 0.1f,
        flowerPetalSpawnDistanceTime = 0.1f, 
        rainCircleSize = 30f,
        rainCycle = 0.5f,
        rainRotateDelay = 0.5f,
        spawnRadius = 18.0f,
        spawnHeightMin = 5.0f,
        rainHeight = 30f;
    [SerializeField]
    int
        rainButterflyNum = 5,
        rainNumAtOneCircle = 10,
        butterflySpawnNumFirst = 10;

    private bool rainFall = true;
    private bool flowerPeatelFall = true;

    // Start is called before the first frame update
    void Start()
    {
        phase = 1;
        butterfly_entry();
        //delay�p�R���[�`���̋N����rain_circle�̋N��
        StartCoroutine(rain_circle());
        phase = 0;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (phase == 1)
        {
            butterfly_entry();
            phase = 0;
        }
    }

    private void butterfly_entry()
    {
        //�����p�x��ݒ肵�ă��W�A���ɕϊ�
        double setaSet = (360 / butterflySpawnNumFirst) * (Math.PI / 180);
        //delay�p�R���[�`���̋N����butterfly_circle�̋N��
        StartCoroutine(butterfly_circle(butterflySpawnNumFirst, butterflySpawnCircleSizeFirst, setaSet, new Vector3(0, 0, 0)));
    }




    //�X�e�[�W�ɒ��𐶐�
    private IEnumerator butterfly_circle(int ButterflyNum, float CircleSize, double setaSet, Vector3 centerCoordinate)
    {
        int i;
        double setaSpawn;
        float spawnX, spawnZ, spawnAngle;
        Vector3 spawnCoordinate;
        for (i = 0; i < ButterflyNum; i++)
        {
            //�������W�ݒ�
            setaSpawn = setaSet * i;
            spawnX = (float)(CircleSize * Math.Cos(setaSpawn));
            spawnZ = (float)(CircleSize * Math.Sin(setaSpawn));
            spawnAngle = (float)(90 + setaSet * i);
            spawnCoordinate = new Vector3(centerCoordinate.x + spawnX, centerCoordinate.y, centerCoordinate.z + spawnZ);
            //���̐����Ɛe�I�u�W�F�N�g��tf��false�ɐݒ�
            var spawnButterfly = Instantiate(butterfly, spawnCoordinate, Quaternion.Euler(0, spawnAngle, 0));
            spawnButterfly.GetComponent<butterfly>().inSmallCage = false;

            yield return new WaitForSeconds(butterflySpawnDistanceTime);
        }
    }

    //rain_cage�ɒ��𐶐�
    private IEnumerator butterfly_circle(int ButterflyNum, float CircleSize, double setaSet, Vector3 centerCoordinate, GameObject parentObject)
    {
        int i;
        double setaSpawn;
        float spawnX, spawnZ, spawnAngle;
        Vector3 spawnCoordinate;
        for (i = 0; i < ButterflyNum; i++)
        {
            //�������W�ݒ�
            setaSpawn = setaSet * i;
            spawnX = (float)(CircleSize * Math.Cos(setaSpawn));
            spawnZ = (float)(CircleSize * Math.Sin(setaSpawn));
            spawnAngle = (float)(90 + setaSet * i);
            spawnCoordinate = new Vector3(centerCoordinate.x + spawnX, centerCoordinate.y, centerCoordinate.z + spawnZ);
            //���̐����Ɛe�I�u�W�F�N�g��tf��true�ɐݒ�
            var spawnButterfly = Instantiate(butterfly, spawnCoordinate, Quaternion.Euler(0, spawnAngle, 0), parentObject.transform);
            spawnButterfly.GetComponent<butterfly>().inSmallCage = true;

            
            yield return new WaitForSeconds(butterflySpawnDistanceTime);
        }
    }

    private void instantiate_particle(Vector3 centerCoordinate, GameObject parentObject)
    {
        Instantiate(particle, centerCoordinate, Quaternion.identity, parentObject.transform);
        Debug.Log("particleInstantiate!");
    } 



    private IEnumerator rain_circle()
    {
        int i;
        float setaSpawn;
        //�����p�x��ݒ肵�ă��W�A���ɕϊ�
        float setaSet = (float)((360 / rainNumAtOneCircle) * (Math.PI / 180));
        float spawnX, spawnZ, angleValue, radiusValue, spawnRadius;
        Vector3 spawnCoordinate;
        while (rainFall)
        {
            for (i = 0; i < rainNumAtOneCircle; i++)
            {
                /*rain_cage���W�ݒ�
                setaSpawn = setaSet * i;
                angleValue = UnityEngine.Random.Range(setaSpawn, setaSpawn + setaSet);
                radiusValue = UnityEngine.Random.Range(0, rainCircleSize);
                spawnX = (float)(radiusValue * Math.Cos(angleValue));
                spawnZ = (float)(radiusValue * Math.Sin(angleValue));
                spawnCoordinate = new Vector3(spawnX, rainHeight, spawnZ);
                */
                spawnCoordinate = Generate_spawn_coordinate();
                //rain_cage�𐶐�
                GameObject smallCage = Instantiate(cage, spawnCoordinate, Quaternion.identity);
                //rain_cage���ɒ��𐶐�
                spawnRadius = (smallCage.transform.GetChild(0).GetComponent<rain_cage_mesh>().scale) / 2;
                StartCoroutine(butterfly_circle(rainButterflyNum, spawnRadius, setaSet, spawnCoordinate, smallCage));
                //particle����
                //instantiate_particle(spawnCoordinate, smallCage);

                yield return new WaitForSeconds(rainRotateDelay);

            }

            yield return new WaitForSeconds(rainCycle);
        }
    }

    /*
    private IEnumerator spawn_flower_petals()
    {
        int i;
        Vector3 spawnCoordinate;
        while (flowerPeatelFall)
        {
            spawnCoordinate = Generate_spawn_coordinate();
            Instantiate(flowerPetal, spawnCoordinate, Quaternion.identity);
            yield return new WaitForSeconds(flowerPetalSpawnDistanceTime);
        }
    }
    */

    private Vector3 Generate_spawn_coordinate()
    {
        Vector3 spawnCoordinate;
        do
        {
            spawnCoordinate = UnityEngine.Random.onUnitSphere * spawnRadius;
        } while (spawnCoordinate.y < spawnHeightMin);

        return spawnCoordinate;
    }
}