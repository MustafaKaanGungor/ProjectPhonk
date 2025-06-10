using System;
using UnityEngine;

public class BOTcarController : MonoBehaviour
{
    private float localVelocityX;
    private float localVelocityZ;
    private Rigidbody carRb;
    private bool isDrifting;

    private void Awake() {
        carRb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        localVelocityX = transform.InverseTransformDirection(carRb.linearVelocity).x;

        if (Input.GetKey(KeyCode.W))
        {

        }
        if(Input.GetKey(KeyCode.S)){
          
        }

        if(Input.GetKey(KeyCode.A)){
          TurnLeft();
        }
        if(Input.GetKey(KeyCode.D)){
          TurnRight();
        }
        if(Input.GetKey(KeyCode.Space)){
          
        }
        if(Input.GetKeyUp(KeyCode.Space)){
          
        }
        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
          
        }
        /*if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
          
        }
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f){
          
        }*/
    }

    private void Accelerate()
    {
        //local velocity x 2.5dan büyükse drift atıyor demek
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        //Araç geriye gidiyorsa önce fren bas, sonra gazla
        if (localVelocityZ < -1)
        {
            Handbrake();
        }
        else
        {

        }
    }

    private void Handbrake()
    {
        
    }

    private void TurnRight()
    {
        throw new NotImplementedException();
    }

    private void TurnLeft()
    {
        throw new NotImplementedException();
    }
}
