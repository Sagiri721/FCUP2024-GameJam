using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPivot : MonoBehaviour
{
    private  Slider instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("hello?");
        instance.value = PlayerController.currentHunger / 100f;
    }
}
