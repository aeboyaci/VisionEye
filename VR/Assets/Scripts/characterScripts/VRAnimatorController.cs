using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAnimatorController : MonoBehaviour
{
    public float speedTreshold = 0.1f;
    private Animator animator;
    private Vector3 previousPos;
    private VRRig vrRig;
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        animator= GetComponent<Animator>();
        vrRig= GetComponent<VRRig>();
        previousPos = vrRig.head.vrTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 headsetSpeed = (vrRig.head.vrTarget.position - previousPos) / Time.deltaTime;
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = vrRig.head.vrTarget.position;
        animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedTreshold);
        //Eger VR da vucut kafa ile beraber donuosa alttaki satir iptal//
        if(headsetLocalSpeed.magnitude > speedTreshold)
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, cam.transform.rotation.eulerAngles.y, transform.rotation.z));


    }
}
