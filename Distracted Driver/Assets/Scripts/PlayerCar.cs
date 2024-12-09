using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCar : MonoBehaviour
{
    [SerializeField] float horizontalSpeed;
    bool isMovingHorizontally = false;
    int lane = 2;
    Stack<Tween> moves = new Stack<Tween>();
    [SerializeField] float verticalSpeed;
    float rightBounds = -2.72f;
    float leftBounds = -6.14f;
    float middleBounds = -4.43f;
    bool stop = false;
    int lives = 1;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.GameOver.AddListener(Stop);
        LaneChange();
    }

    // Update is called once per frame
    void Update()
    {

        if (!stop)
        {
            MoveHorizontal(Input.GetAxis("Horizontal"));
            MoveVertical(Input.GetAxis("Vertical"));
        }

    }

    //Gets horizontal input and changes lane number, then moves car lanes
    void MoveHorizontal(float input)
    {
        //if left input
        if (input == -1)
        {
            LaneNumChange(input);
            LaneChange();
        }
        //if right input
        else if (input == 1)
        {
            LaneNumChange(input);
            LaneChange();
        }
        else if(input == 0)
        {
            SetMovingHorizontally(false);
        }
    }

    //if car is not moving horizontally, change lane number
    void LaneNumChange(float input)
    {
        if (!isMovingHorizontally)
        {
            int intput = (int)input;
            lane = Clamp3(lane + intput);
        }
    }

    //this is called after lane number is changed
    void LaneChange()
    {
        //if in lane number set to lane 1, move to lane one in horizontalSpeed seconds, upda    tes whether moving horizontally or not
        if (lane == 1)
        {
            transform.DOMoveX(leftBounds, horizontalSpeed)
                .OnStart(() => SetMovingHorizontally(true))
                .SetEase(Ease.OutSine)
                .OnComplete(() => SetMovingHorizontally(false));
        }
        //same as lane 1, but for lane 2
        else if (lane == 2)
        {
            transform.DOMoveX(middleBounds, horizontalSpeed)
                .OnStart(() => SetMovingHorizontally(true))
                .SetEase(Ease.OutSine)
                .OnComplete(() => SetMovingHorizontally(false));
        }
        //same as lane 1, but for lane 3
        else
        {
            transform.DOMoveX(rightBounds, horizontalSpeed)
                .OnStart(() => SetMovingHorizontally(true))
                .SetEase(Ease.OutSine)
                .OnComplete(() => SetMovingHorizontally(false));
        }

    }

    void SetMovingHorizontally(bool bol)
    {
        isMovingHorizontally = bol;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            lives--;
            if (lives <= 0)
            {
                EventManager.GameOver.Invoke();
            }
            
        }

    }

   void Stop()
   {
        stop = true;
   }


}
