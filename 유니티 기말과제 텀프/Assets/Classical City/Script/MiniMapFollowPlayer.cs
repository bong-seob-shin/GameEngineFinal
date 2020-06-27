using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }

}
