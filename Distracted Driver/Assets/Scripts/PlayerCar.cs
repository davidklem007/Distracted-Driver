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
        //Debug.Log("axis: " + Input.GetAxis("Horizontal"));
        //Debug.Log("newPos: " + newPos + "\ncurPos: " + transform.position);


    }

    //Moves lane by lane, bounded by lanes
    //Has to switch lanes before another input is accepted
    //First input has to be released for new input to be detected
    void MoveHorizontal(float input)
    {
        if (input == -1 && horizontalPressed)
        {
            if (!left && !right)
            {
                oldPos = transform.position;
                newPos = new Vector3(transform.position.x - 1.70f, transform.position.y, transform.position.z);
            }
            left = true;
            horizontalPressed = false;
        }
        else if (input == 1 && horizontalPressed)
        {
            if (!right && !left)
            {
                oldPos = transform.position;
                newPos = new Vector3(transform.position.x + 1.70f, transform.position.y, transform.position.z);
            }
            right = true;
            horizontalPressed = false;
        }
        else if(input == 0)
        {
            horizontalPressed = true;
        }

        if (left)
        {
            if(!(transform.position.x <= newPos.x) && transform.position.x >= -6.15)
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
}
