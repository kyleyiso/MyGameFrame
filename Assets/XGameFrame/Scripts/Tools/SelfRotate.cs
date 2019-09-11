using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    public Vector3 DirVector3;
    public float RoateSpeed;
	void Update ()
	{
		transform.Rotate(DirVector3,Time.deltaTime*RoateSpeed);
	}
}
