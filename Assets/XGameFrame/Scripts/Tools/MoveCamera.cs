using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour
{
    //在摄像机前实例一个物体，每次移动该物体
    private Transform targetCamerTrans;

    public Transform TargetCamerTrans
    {
        get
        {
            if (targetCamerTrans == null)
            {
                targetCamerTrans = new GameObject("TargetTrans").transform;
                targetCamerTrans.position = CameraManager.Instance.MainCameraTrans.position;
                targetCamerTrans.rotation = CameraManager.Instance.MainCameraTrans.rotation;
            }
            return targetCamerTrans;
        }
    }


    private static float Smooth = 6f;//缓动
    private Vector3 SmoothVec = Vector3.one * Smooth;
    private float vo = 0.25f;

    private Touch CurentTouch;
    private float SpeedX;
    private float SpeedY;
    /// <summary>
    /// 触屏移动速度
    /// </summary>
    public float MoveRate = 0.002f;

    public float RightMax_PosX;//右边最大边界X
    public float LeftMin_PosX;//左边最小边界X
    public float TopMax_PosY;//上面最大边界Y
    public float BottomMin_PosY;//下面最小边界Y

    private Vector2 lastPos;

    void FixedUpdate()
    {

#if !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
        }
        MoveCameraByTouch();
#else
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        }

        MoveCameraByMouse();
#endif
    }

    //使用触屏移动
    private void MoveCameraByTouch()
    {
        if (CameraManager.CanMoveCarmer)
        {
            if (Input.touchCount == 1)
            {
                CurentTouch = Input.touches[0];
                if (CurentTouch.phase == TouchPhase.Began)
                {
                    lastPos = CurentTouch.position;
                }
                else if (CurentTouch.phase == TouchPhase.Moved && CurentTouch.phase != TouchPhase.Stationary)
                {
                    if (lastPos != CurentTouch.position)//解决手指用力向下按也会触发TouchPhase.Moved
                    {
                        SpeedX = Mathf.Abs(CurentTouch.deltaPosition.x / CurentTouch.deltaTime * MoveRate);
                        SpeedY = Mathf.Abs(CurentTouch.deltaPosition.y / CurentTouch.deltaTime * MoveRate);
                        TargetCamerTrans.Translate(new Vector3(CurentTouch.deltaPosition.x * SpeedX, 0, CurentTouch.deltaPosition.y * SpeedY)
                                                   * Time.deltaTime, Space.World);
                    }
                    lastPos = CurentTouch.position;
                }
            }
        }
        
        //缓动
        if (Vector3.Distance(CameraManager.Instance.MainCameraTrans.position, TargetCamerTrans.position) > 0.005f)
        {
            CameraManager.Instance.MainCameraTrans.position = Vector3.SmoothDamp(CameraManager.Instance.MainCameraTrans.position
                , TargetCamerTrans.position, ref SmoothVec, vo);
            //限制区域
            TargetCamerTrans.position = new Vector3(Mathf.Clamp(TargetCamerTrans.position.x, RightMax_PosX, LeftMin_PosX),
                TargetCamerTrans.position.y, Mathf.Clamp(TargetCamerTrans.position.z, TopMax_PosY, BottomMin_PosY)
            );
        }
    }
    
    private void MoveCameraByMouse()
    {
        if (Input.GetMouseButton(0) && CameraManager.CanMoveCarmer)
        {
            float mouseX = Input.GetAxis("Mouse X") ;
     
            float mouseY = Input.GetAxis("Mouse Y") ;

            TargetCamerTrans.Translate(new Vector3(mouseX, 0, mouseY), Space.World);
        }

        if (Vector3.Distance(CameraManager.Instance.MainCameraTrans.position, TargetCamerTrans.position) > 0.005f)
        {
            CameraManager.Instance.MainCameraTrans.position = Vector3.SmoothDamp(CameraManager.Instance.MainCameraTrans.position
                , TargetCamerTrans.position, ref SmoothVec, vo);
            //限制区域
            TargetCamerTrans.position = new Vector3(Mathf.Clamp(TargetCamerTrans.position.x, RightMax_PosX, LeftMin_PosX),
                TargetCamerTrans.position.y, Mathf.Clamp(TargetCamerTrans.position.z, TopMax_PosY, BottomMin_PosY)
            );
        }
    }
}
