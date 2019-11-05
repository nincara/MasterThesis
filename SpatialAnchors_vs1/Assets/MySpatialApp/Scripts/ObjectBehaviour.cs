using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectBehaviour : MonoBehaviour
{
    public float rotationZ; 
    // Start is called before the first frame update
    void Start()
    {
        rotationZ = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the cube by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(-90, rotationZ, 0);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * 5f);

        rotationZ = rotationZ + 1;
        if (rotationZ >= 360) {
            rotationZ = 0;
        }
    }
}
