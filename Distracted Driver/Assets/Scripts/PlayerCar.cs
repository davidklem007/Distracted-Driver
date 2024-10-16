using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    [SerializeField] float horizontalSpeed;
    bool horizontalPressed = false;
    bool left = false;
    bool right = false;
    Vector3 oldPos;
    Vector3 newPos;
    [SerializeField] float verticalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        oldPos = transform.position;
        newPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontal(Input.GetAxis("Horizontal"));
        MoveVertical(Input.GetAxis("Vertical"));
        //Debug.Log("axis: " + Input.GetAxis("Horizontal"));
        //Debug.Log("newPos: " + newPos + "\ncurPos: " + transform.position);


    }

    void FixedUpdate()
    {
        if(transform.position.x < -6.15)
        {
            transform.position = new Vector3(-6.15f, transform.position.y, transform.position.z);
            left = false;
        }
        else if(transform.position.x > -2.75)
        {
            transform.position = new Vector3(-2.75f, transform.position.y, transform.position.z);
            right = false;
        }
    }

    //Moves lane by lane, bounded by lanes
    //Has to switch lanes before another input is accepted
    //First input has to be released for new input to be detected
    void MoveHorizontal(float input)
    {
        if (input == -1 && horizontalPressed)
        {
            if (!left && !right && transform.position.x > -6.15)
            {
                oldPos = transform.position;
                newPos = new Vector3(transform.position.x - 1.70f, transform.position.y, transform.position.z);
            }
            left = true;
            horizontalPressed = false;
        }
        else if (input == 1 && horizontalPressed && transform.position.x < -2.75)
        {
            if (!right && !left)
            {
                oldPos = transform.position;
                newPos = new Vector3(transform.position.x + 1.70f, transform.position.y, transform.position.z);
            }
            right = true;
            horizontalPressed = false;
        }
        else if (input == 0)
        {
            horizontalPressed = true;
        }
        if (left)
        {
            if (!(transform.position.x <= newPos.x) && transform.position.x >= -6.15)
            {
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
            }
            else
            {
                left = false;
            }
        }
        else if (right)
        {
            if (!(transform.position.x >= newPos.x) && transform.position.x <= -2.75)
            {
                transform.Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
            }
            else
            {
                right = false;
            }
        }
    }

    void MoveVertical(float input)
    {
        if(transform.position.y >= -4.3f && transform.position.y <= 4.3f)
        {
            transform.Translate(input * Vector2.up * verticalSpeed * Time.deltaTime);
        }
        else if(transform.position.y < -4.3f)
        {
            transform.position = new Vector3(transform.position.x, -4.3f, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 4.3f, transform.position.z);
        }
        
    }
}
