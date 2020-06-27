using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y+2, player.transform.position.z);
        transform.position = playerPos;
    }
}
