using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    [SerializeField] float horizontalSpeed;
    bool horizontalPressed = false;
    bool left = false;
    bool right = false;
    int lane = 3;
    Vector3 oldPos;
    Vector3 newPos;
    [SerializeField] float verticalSpeed;
    float rightBounds = -2.72f;
    float leftBounds = -6.14f;
    float middleBounds = -4.43f;

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
        LaneChange();
        MoveVertical(Input.GetAxis("Vertical"));
        //Debug.Log("axis: " + Input.GetAxis("Horizontal"));
        //Debug.Log("newPos: " + newPos + "\ncurPos: " + transform.position);


    }

    //Works with LaneChange()
    //Moves lane by lane, bounded by lanes
    //Has to switch lanes before another input is accepted
    //MoveHorizontal() gets the input, and changes lane number
    //First input has to be released for new input to be detected
    void MoveHorizontal(float input)
    {
        if (input == -1 && horizontalPressed && transform.position.x > leftBounds)
        {
            if (!right)
            {
                left = true;
                LaneNumChange(input);
            }
            horizontalPressed = false;
        }
        else if (input == 1 && horizontalPressed && transform.position.x < rightBounds)
        {
            if (!left)
            {
                right = true;
                LaneNumChange(input);
            }
            horizontalPressed = false;
        }
        else if (input == 0)
        {
            horizontalPressed = true;
        }
    }

    //Changes lane based on lane number and if input was recieved
    //Assures that car ends up in correct position corresponding to the lane
    void LaneChange()
    {
        //if lane number is 1 and left input recieved
        //if car reached the correct lane position or went too far left, turn off left input and set to correct lane position
        //if not at lane position yet, move left
        //if left input off, if car is not in correct lane position, move to the correct position
        if (lane == 1)
        {
            if (left)
            {
                if (transform.position.x <= leftBounds)
                {
                    left = false;
                    transform.position = new Vector3(leftBounds, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
                }
            }
            else
            {
                if (transform.position.x != leftBounds)
                {
                    left = false;
                    transform.position = new Vector3(leftBounds, transform.position.y, transform.position.z);
                }
            }
        }
        //Like a combination of checking for lane 1 and 3 but with lane 2 bounds
        //first checks left input, the if no left input, checks right input
        //if no input at all, assures car is in right position
        else if (lane == 2)
        {
            if (left)
            {
                if (transform.position.x <= middleBounds)
                {
                    left = false;
                    transform.position = new Vector3(middleBounds, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
                }
            }
            else if (right)
            {
                if (transform.position.x >= middleBounds)
                {
                    right = false;
                    transform.position = new Vector3(middleBounds, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
                }
            }
            else
            {
                if (transform.position.x != middleBounds)
                {
                    left = false;
                    right = false;
                    transform.position = new Vector3(middleBounds, transform.position.y, transform.position.z);
                }
            }
        }
        //Lane 3, same as lane 1 check but instead checks for right input with the lane 3 bounds
        else
        {
            if (right)
            {
                if (transform.position.x >= rightBounds)
                {
                    right = false;
                    transform.position = new Vector3(rightBounds, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
                }
            }
            else
            {
                if (transform.position.x != rightBounds)
                {
                    right = false;
                    transform.position = new Vector3(rightBounds, transform.position.y, transform.position.z);
                }
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

    void LaneNumChange(float input)
    {
        int intput = (int) input;
        lane = Clamp3(lane + intput);
    }

    int Clamp3(int num)
    {
        if (num > 3)
        {
            return 3;
        }
        if(num < 1)
        {
            return 1;
        }
        return num;

    }

    
}
