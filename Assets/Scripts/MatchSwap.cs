using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSwap : MatchMath
{
    private GameObject oriMatch, tarLocation;
    private Vector3 tempPos, tempAngles;
   public Transform oriParent, tarParent;

    private bool t2 = true;
    private bool t1 = true;

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0f;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //referencing from website: github.com/Syphon/Unity3D/issues/5  --> ScreenPointToRay(Input.mousePosition) is always problematic with orthographic camer
            //creating modifying factors to manuelly calculate the world position
            Vector2 camToScreenFactor = new Vector2(Camera.main.pixelWidth / Screen.width, Camera.main.pixelHeight / Screen.height);
            Vector3 camToScreenPos = new Vector3(Input.mousePosition.x * camToScreenFactor.x, Input.mousePosition.y * camToScreenFactor.y, Camera.main.nearClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(camToScreenPos);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Match")
                {
                    oriMatch = hit.collider.gameObject;
                    tempPos = oriMatch.transform.position;
                    tempAngles = oriMatch.transform.localEulerAngles;
                    oriParent = oriMatch.transform.parent;
                    Rigidbody rb = oriMatch.gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = enabled;
                    t1 = false;
                }
                else
                    return;
            }
        }
        if (t1 == false)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 camToScreenFactor = new Vector2(Camera.main.pixelWidth / Screen.width, Camera.main.pixelHeight / Screen.height);
                Vector3 camToScreenPos = new Vector3(Input.mousePosition.x * camToScreenFactor.x, Input.mousePosition.y * camToScreenFactor.y, Camera.main.nearClipPlane);
                Ray ray = Camera.main.ScreenPointToRay(camToScreenPos);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "Empty" && t1 == false)
                    {
                        tarLocation = hit.collider.gameObject;
                        tarParent = tarLocation.transform.parent;

                        t2 = false;
                    }

                    else
                    {
                        oriMatch.transform.position = tempPos;
                        oriMatch.transform.localEulerAngles = tempAngles;
                        t1 = true;
                    }
                }
            }
        }

        if (t1 == false && t2 == false)
        {
            Swap(oriMatch, tarLocation);
        }
    }

     void Swap(GameObject initial, GameObject tar)
     {
        //release from parent
        initial.transform.parent = null;
        tar.transform.parent = null;

        //reassign transform
        initial.transform.position = tar.transform.position;
        initial.transform.localEulerAngles = tar.transform.localEulerAngles;
        tar.transform.position = tempPos;
        tar.transform.localEulerAngles = tempAngles;

        //assign parent
        initial.transform.parent = tarParent;
        tar.transform.parent = oriParent;

        t1 = true;
        t2 = true;
    }
}
