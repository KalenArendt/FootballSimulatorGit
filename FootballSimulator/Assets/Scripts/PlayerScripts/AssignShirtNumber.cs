using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class AssignShirtNumber : MonoBehaviour
{
    // Shirt Number
    public int ShirtNumber;

    void Start()
    {
        transform.GetComponentInChildren<TextMeshProUGUI>().text = ShirtNumber.ToString();
    }

    // Start is called before the first frame update
    void Update()
    {
        if (Application.isEditor)
        {
            transform.GetComponentInChildren<TextMeshProUGUI>().text = ShirtNumber.ToString();
        }
    }
}
