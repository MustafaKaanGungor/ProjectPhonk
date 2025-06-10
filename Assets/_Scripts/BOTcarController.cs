using System;
using UnityEngine;

public class BOTcarController : MonoBehaviour
{
    [SerializeField] private int accelerationMultiplier = 2;
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    private float carSpeed;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float maxReverseSpeed = 10f;
    [SerializeField] private float brakeForce = 5f;
    private float localVelocityX;
    private float localVelocityZ;
    private Rigidbody carRb;
    private bool isDrifting;
    private float throttleAxis;
    private float steeringAxis;
    [SerializeField] private float steeringSpeed = 5f;
    [SerializeField] private float maxSteeringAngle = 30f;

    private void Awake()
    {
        carRb = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }

    void Update()
    {
        carSpeed = 2 * Mathf.PI * frontLeftWheelCollider.radius * frontLeftWheelCollider.rpm * 60 / 1000;
        Debug.Log(carSpeed);
        localVelocityX = transform.InverseTransformDirection(carRb.linearVelocity).x;

        if (Input.GetKey(KeyCode.W))
        {
            GoForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            GoReverse();
        }
        if (Input.GetKey(KeyCode.A))
        {
            TurnLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            TurnRight();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Handbrake();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {

        }
        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            ThrottleOff();
        }
        /*if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
            Decelerate();
        }*/
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
        {
            ResetSteering();
        }
    }

    private void GoForward()
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

        //Gazı bir anda değil yavaşça ver ki pedala basıyormuş gibi olsun
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }

        //Araç geriye gidiyorsa önce fren bas, sonra gazla
        if (localVelocityZ < -1)
        {
            Handbrake();
        }
        else
        {
            if (carSpeed < maxSpeed)
            {  //aracın hızı maksdan az ise gaz ver
                rearLeftWheelCollider.motorTorque = accelerationMultiplier * throttleAxis;
                rearRightWheelCollider.motorTorque = accelerationMultiplier * throttleAxis;
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
                frontLeftWheelCollider.brakeTorque = 0;
                frontRightWheelCollider.brakeTorque = 0;
            }
            else
            { //Aracın hızı maksimumdan yüksekse daha fazla gaz verme
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }

        }
    }

    private void GoReverse()
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

        //Gazı bir anda değil yavaşça ver ki pedala basıyormuş gibi olsun
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }

        //Araç ileri gidiyorsa önce fren bas, sonra gazla
        if (localVelocityZ > 1f)
        {
            Handbrake();
        }
        else
        {
            if (carSpeed > -maxReverseSpeed)
            { //aracın hızı maksdan az ise gaz ver
                rearLeftWheelCollider.motorTorque = accelerationMultiplier * throttleAxis;
                rearRightWheelCollider.motorTorque = accelerationMultiplier * throttleAxis;
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
                frontLeftWheelCollider.brakeTorque = 0;
                frontRightWheelCollider.brakeTorque = 0;
            }
            else
            { //Aracın hızı maksimumdan yüksekse daha fazla gaz verme
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }

        }
    }

    private void Handbrake()
    {
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    private void ThrottleOff()
    {
        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
        rearLeftWheelCollider.motorTorque = 0;
        rearRightWheelCollider.motorTorque = 0;
    }

    private void Decelerate()
    {

    }

    private void TurnRight()
    {
        //Direksiyonu yavaşça sağa çevir
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }

        //Direksiyon açısını tekerlere uygula
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void TurnLeft()
    {
        //Direksiyonu yavaşça sola çevir
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }

        //Direksiyon açısını tekerlere uygula
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void ResetSteering()
    {
        //Direksiyon açısını zamanla normale döndür
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(frontLeftWheelCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        
        //Direksiyon açısını tekerlere uygula
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, steeringAngle, steeringSpeed);
    }
}
