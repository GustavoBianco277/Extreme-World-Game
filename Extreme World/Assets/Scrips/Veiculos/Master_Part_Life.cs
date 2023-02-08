using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Master_Part_Life : MonoBehaviour
{
    public float Life = 3000;
    private float MaxLife;
    public Slider MasterLife;
    public Image Fill;
    private Helicopter_Controller Heli;
    void Start()
    {
        Heli = GetComponent<Helicopter_Controller>();
        MaxLife = Life;
        float value = MasterLife.value = 1 / MaxLife * Life;
        Fill.color = new Color(1 - value, value, 0);
    }

    void Update()
    {
        
    }

    public void Death(float Damage)
    {
        Life -= Damage;
        SliderColor();

        if (Life <= 0 && Heli.Controller)
        {
            Heli.ExplosaoVoid();
        }
    }
    private void SliderColor()
    {
        float value = 1 / MaxLife * Life;
        MasterLife.value = value;
        if (value > 0.5f)
            Fill.color = new Color((1 - value) * 2, 1, 0);
        else
            Fill.color = new Color(1, value * 2, 0);
        if (value <= 0.01f)
            Fill.enabled = false;
    }
}
