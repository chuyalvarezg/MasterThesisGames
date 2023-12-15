using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Camera camera;
    public Camera originCamera;

    // Start is called before the first frame update
    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        camera.fieldOfView = originCamera.fieldOfView;
        camera.transform.position = originCamera.transform.position;
        camera.transform.rotation = originCamera.transform.rotation;
    }
}
