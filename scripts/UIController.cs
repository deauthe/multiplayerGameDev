using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Slider weaponTempSlider;
    public void Awake()
    {
        instance = this;
    }
    public TMP_Text overHeatedMessage;
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
