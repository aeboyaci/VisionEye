using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class slidingPuzzle : MonoBehaviour
{
    int x;
    int y;
    int z;
    Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
         position = transform.position;
         x = (int)position.x;
         y = (int)position.y;
         z = (int)position.z;


    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        int a = (int)position.x;
        int b = (int)position.y;
        int c = (int)position.z;
        if((a!=x) || (b != y) || (c != z))
        {
            print(x+" "+y+" "+z);
            print(position.x + " " + position.y + " " + position.z);
        }
    }
}
