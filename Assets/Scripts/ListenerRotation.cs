using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerRotation : MonoBehaviour
{
    GameObject player; //プレイヤー情報

    public float followSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        if (player != null)
        {
            //線形補間を使って、カメラを目的の場所に動かす
            //Lerp(今の位置、ゴールとすべき位置、割合)
            transform.rotation =
                Quaternion.Lerp(transform.rotation, player.transform.rotation, followSpeed * Time.deltaTime);
        }
    }

}
