using UnityEngine;

public class DragMatch : MatchMath
{
    private Vector3 mOffset;

    Rigidbody rb;

    private void Update()
    {
        rb = GetComponent<Rigidbody>();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector2 camToScreenFactor = new Vector2(Camera.main.pixelWidth / Screen.width, Camera.main.pixelHeight / Screen.height);
        Vector3 camToScreenPos = new Vector3(Input.mousePosition.x * camToScreenFactor.x, Input.mousePosition.y * camToScreenFactor.y, 0);

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(camToScreenPos);
    }

    void OnMouseDown()  
    {
        // Store offset = gameobject world pos - mouse world pos
        mOffset = transform.position - GetMouseAsWorldPoint();
        
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    private void OnMouseUp()
    {
        Destroy(rb);
    }
}

//Note 1: OnMouseDown() and similar functions do not work when multiple colliders stack together.
//Note 2: trigger is not necessary for above functions.