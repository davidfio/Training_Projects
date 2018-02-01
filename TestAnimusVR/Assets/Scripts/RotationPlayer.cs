using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPlayer : MonoBehaviour
{

	void Update ()
    {
        this.transform.rotation = this.GetComponentInChildren<Transform>().rotation;
	}
}
