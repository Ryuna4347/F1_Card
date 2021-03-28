using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 playerPos = Vector2.Lerp(transform.position, player.transform.position, 0.1f);
        //transform.position = new Vector3(playerPos.x, playerPos.y, -10f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
