using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPreview : MonoBehaviour
{

    private void OnTriggerStay (Collider other)
    {
        if (other.gameObject.tag == "Match")
            other.transform.localEulerAngles = transform.localEulerAngles;
    }
}
