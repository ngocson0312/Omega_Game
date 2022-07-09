using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] private Rigidbody _rg;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private float _moveSpeed;
    void Update()
    {
        //chạm vào để hiển thị nút di chuyển
        Touch();
    }

    public void Touch()
    {

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PlayerMove();
        }
        else
        {
            _rg.velocity = new Vector3(_joystick.Horizontal, _rg.velocity.y, _joystick.Vertical);

        }

    }

    public void PlayerMove()
    {
        // di chuyển nhân vật
        _rg.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, _rg.velocity.y, _joystick.Vertical * _moveSpeed);

        //Kiểm tra xem nhân vật có di chuyển hay không?
        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            //code khả năng xoay người của nhân vật
            transform.rotation = Quaternion.LookRotation(_rg.velocity);
        }
    }
}
