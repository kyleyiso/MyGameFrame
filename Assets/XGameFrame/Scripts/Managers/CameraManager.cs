using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// 3D摄像机
    /// </summary>
    public Camera MainCamera;
    public Transform MainCameraTrans;
    /// <summary>
    /// UI摄像机
    /// </summary>
    public Transform UICameraTrans;

    private static CameraManager instance;
    public static CameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("Camers").GetComponent<CameraManager>();
            }

            return instance;
        }
    }

    public static Vector3 OrgCameraPos= new Vector3(-110f, 150f, 140f);

    private MoveCamera moveCamera;
    private MoveCamera _MoveCamera
    {
        get
        {
            if (moveCamera == null)
            {
                moveCamera = MainCameraTrans.GetComponent<MoveCamera>();
            }
            return moveCamera;
        }
    }

    
    public static bool CanMoveCarmer = true;//能否移动摄像机
    public void InitCarmerPostion()
    {
        SetCameraMaxLeftPostion();
        SetCarmerPostion(false);
    }
    /// <summary>
    /// 设置摄像机左边最大的移动距离
    /// </summary>
    public void SetCameraMaxLeftPostion()
    {

    }

    public void SetCarmerPostion(bool IsNeedAnimation)
    {
        if (IsNeedAnimation)
        {
            //_MoveCamera.TargetCamerTrans
            //    .DOMove( new Vector3(OrgCameraPos.x + (PlayerManager._PlayerData.SelfHousePropertyList.Count - 1) * 41.5f, OrgCameraPos.y, OrgCameraPos.z),
            //        0.45f).SetEase(Ease.Linear);
        }
        else
        {
            //MainCameraTrans.position=_MoveCamera.TargetCamerTrans.position = new Vector3(OrgCameraPos.x + (PlayerManager._PlayerData.SelfHousePropertyList.Count - 1) * 41.5f,
            //    OrgCameraPos.y, OrgCameraPos.z);

        }
    }
}
