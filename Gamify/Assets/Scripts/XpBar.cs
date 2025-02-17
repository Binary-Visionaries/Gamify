using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    private Slider slider;
    private ParticleSystem particlSys1,particlSys2;

    public float FillSpeed = 0.4f;
    private float targetProgress = 0;
    private float remainingProgress = 0;

    public Animator anim;

    private void Awake(){
        slider = gameObject.GetComponent<Slider>();
        particlSys1 = GameObject.Find("Xp Particles1").GetComponent<ParticleSystem>();
        particlSys2 = GameObject.Find("Xp Particles2").GetComponent<ParticleSystem>();

    }

    void Start()
    {
        // Testing animation
        //IncrementProgress(2.5f);
    }

    void Update()
    {
        // If there's progress to animate
        if (slider.value < targetProgress)
        {
            slider.value += FillSpeed * Time.deltaTime;

            // Trigger level-up animation if slider reaches full
            if (slider.value >= 1.0f)
            {
                anim.SetTrigger("LevelUp");
                slider.value = 0; // Reset the bar for the next increment
                targetProgress -= 1.0f; // Reduce the target by 1
            }

            // Play particle effects if they're not playing
            if (!particlSys1.isPlaying || !particlSys2.isPlaying)
            {
                particlSys1.Play();
                particlSys2.Play();
            }
        }
        else
        {
            // Stop particle effects when animation completes
            particlSys1.Stop();
            particlSys2.Stop();
        }
    }

    public void IncrementProgress(float newProgress)
    {
        // Add new progress to the remaining target
        targetProgress = slider.value + newProgress;
    }
}