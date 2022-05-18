using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//这个UI管界面上的两个按键
public class gobangUI : MonoBehaviour
{
    public Button restart;
    public Button repentance;
    public bool restartEnabled;
    public bool repentanceEnabled;
    
    public void restartClick()
    {
        this.restartEnabled = true;
    }

    public void repentanceClick()
    {
        this.repentanceEnabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.restartEnabled = false;
        this.repentanceEnabled = false;
        restart.onClick.AddListener(() =>
        {
            restartClick();
        });
        repentance.onClick.AddListener(() =>
        {
            repentanceClick();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
