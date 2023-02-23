using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public KeyCode front = KeyCode.Z;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.Q;
    public KeyCode right = KeyCode.D;
    public CarController cc;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(front))
        {
            cc.front = true;
        }
        else if (Input.GetKey(back))
        {
            cc.back = true;
        }
        else
        {
            cc.front = false;
            cc.back = false;
        }
        
        if (Input.GetKey(left))
        {
            cc.left = true;
        }
        else if (Input.GetKey(right))
        {
            cc.right = true;
        }
        else
        {
            cc.left = false;
            cc.right = false;
        }
    }
}
