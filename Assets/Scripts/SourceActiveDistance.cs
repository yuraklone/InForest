using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceActiveDistance : MonoBehaviour
{
    public GameObject audioSource;
    public GameObject sourceArea;
    GameObject player;
    public float activeDis = 200f; 


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sourcePos = sourceArea.transform.position;
        Vector3 playerPos = player.transform.position;

        float dis = Vector3.Distance(sourcePos, playerPos);

        if(dis <= activeDis)
        {
            audioSource.SetActive(true);
        }
        else
        {
            audioSource.SetActive(false);
        }
    }
}
