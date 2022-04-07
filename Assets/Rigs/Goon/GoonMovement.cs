using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class GoonMovement : MonoBehaviour
{
    enum Mode
    {
        Idle,
        Walk
    }

    public FootRaycast footLeft;

    public FootRaycast footRight;

    public float speed = 2;

    public float walkSpreadX = .2f;

    public float walkSpreadY = .4f;

    public float walkSpreadZ = .8f;

    public float walkFootSpeed = 4;

    private CharacterController pawn;

    private Mode mode = Mode.Idle;

    private Vector3 input;

    private float walkTime;

    private Camera cam;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = Vector3.Cross(Vector3.up, camForward);

        input = camForward * v + camRight * h;
        if (input.sqrMagnitude > 1) input.Normalize();

        // set movement mode based on movement input:
        float threshold = .1f;
        mode = (input.sqrMagnitude > threshold * threshold) ? Mode.Walk : Mode.Idle;

        pawn.SimpleMove(input * speed);

        Animate();
    }
    void Animate()
    {
        switch (mode)
        {
            case Mode.Idle:
                AnimateIdle();
                break;
            case Mode.Walk:
                AnimateWalk();
                break;
        }
    }
    void AnimateIdle()
    {
        footLeft.SetPositionHome();
        footRight.SetPositionHome();

    }

    delegate void MoveFoot(float time, FootRaycast foot);
    void AnimateWalk()
    {
        MoveFoot moveFoot = (t, foot) => 
        {
            float y = Mathf.Cos(t) * walkSpreadY; // vertical movement
            float lateral = Mathf.Sin(t) * walkSpreadZ; // lateral movement

            Vector3 localDir = foot.transform.parent.InverseTransformDirection(input);

            float x = lateral * localDir.x;
            float z = lateral * localDir.z;

            if (y < 0) y = 0;

            foot.SetPositionOffset(new Vector3(x, y, z));


        };

        walkTime += Time.deltaTime * input.sqrMagnitude * walkFootSpeed;

        moveFoot.Invoke(walkTime, footLeft);
        moveFoot.Invoke(walkTime + Mathf.PI, footRight);


    }



}
