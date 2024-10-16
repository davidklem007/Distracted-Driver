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
    float leftBounds = -6.12f;
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
        MoveVertical(Input.GetAxis("Vertical"));
        //Debug.Log("axis: " + Input.GetAxis("Horizontal"));
        //Debug.Log("newPos: " + newPos + "\ncurPos: " + transform.position);


    }

    void FixedUpdate()
    {
        if(transform.position.x < -6.15)
        {
            transform.position = new Vector3(leftBounds, transform.position.y, transform.position.z);
            left = false;
        }
        else if(transform.position.x > -2.75)
        {
            transform.position = new Vector3(rightBounds, transform.position.y, transform.position.z);
            right = false;
        }
    }

    //Moves lane by lane, bounded by lanes
    //Has to switch lanes before another input is accepted
    //First input has to be released for new input to be detected
    void MoveHorizontal(float input)
    {
        if (input == -1 && horizontalPressed && transform.position.x > leftBounds)
        {
            if (!left && !right)
            {
                oldPos = transform.position;
                newPos = new Vector3(transform.position.x - 1.70f, transform.position.y, transform.position.z);
            }
            left = true;
            horizontalPressed = false;
        }
        else if (input == 1 && horizontalPressed && transform.position.x < rightBounds)
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
            if (!(transform.position.x <= newPos.x) && transform.position.x >= leftBounds)
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
            if (!(transform.position.x >= newPos.x) && transform.position.x <= rightBounds)
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

    void LaneChange(float input)
    {
        int intput = Mathf.RoundToInt(input);
        lane = Clamp3(intput + lane);
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

    void LaneClamp()
    {
        if(lane == 1)
        {
            if (left)
            {
                if(transform.position.x < leftBounds)
                {
                    left = false;
                    transform.position = new Vector3(leftBounds, transform.position.y, transform.position.z);
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
        else if(lane == 2)
        {
            if (left)
            {
                if (transform.position.x < middleBounds)
                {
                    left = false;
                    transform.position = new Vector3(middleBounds, transform.position.y, transform.position.z);
                }
            }
            else if(right)
            {
                if (transform.position.x > middleBounds)
                {
                    right = false;
                    transform.position = new Vector3(middleBounds, transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (transform.position.x != leftBounds)
                {
                    left = false;
                    right = false;
                    transform.position = new Vector3(leftBounds, transform.position.y, transform.position.z);
                }
            }
        }
        else
        {
            if (right)
            {
                if (transform.position.x > rightBounds)
                {
                    right = false;
                    transform.position = new Vector3(rightBounds, transform.position.y, transform.position.z);
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
}
