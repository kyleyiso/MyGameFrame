using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameTools
{
    /// <summary>
    /// 场景中的物体始终朝向3D摄像机
    /// </summary>
    public class LookAtCamera : MonoBehaviour
    {
        void Update()
        {
            transform.forward = CameraManager.Instance.MainCameraTrans.forward;
        }
    }
}

