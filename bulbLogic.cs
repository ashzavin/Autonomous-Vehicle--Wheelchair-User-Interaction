using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulbLogic : MonoBehaviour
{
    public Material[] material;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Changes the colour of light between red (notSafeToCross) and green (SafeToCross)
    public void SetCrosswalkLight(int safeToCross)
    {
        rend.sharedMaterial = material[safeToCross];
    }
}
