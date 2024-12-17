using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Image targetImage;
    int starting;
    public Sprite original;
    public Sprite second;
    public Sprite third;

    // Start is called before the first frame update
    void Start()
    {
        starting = Boids.numberOfSphere;
    }

    // Update is called once per frame
    void Update()
    {
        if(Boids.numberOfSphere - Die.beesdie < (Boids.numberOfSphere / 4))
        {
            targetImage.sprite = third;
        }
        else if(Boids.numberOfSphere - Die.beesdie < (Boids.numberOfSphere / 2))
        {
            targetImage.sprite = second;
        }
        else if(Boids.numberOfSphere - Die.beesdie > (Boids.numberOfSphere / 2))
        {
            targetImage.sprite = original;
        }
    }
}
