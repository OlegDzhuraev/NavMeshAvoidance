using UnityEngine;

public class CameraControls : MonoBehaviour
{
    const int MouseButton = 0;

    Vector3 prevPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(MouseButton))
            prevPos = Input.mousePosition;

        if (Input.GetMouseButton(MouseButton))
        {
            var delta = new Vector3(Input.mousePosition.x - prevPos.x, 0, Input.mousePosition.y - prevPos.y);
            transform.position += delta * Time.smoothDeltaTime;
            prevPos = Input.mousePosition;
        }
    }
}
