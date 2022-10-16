using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public ParticleSystem paintSplash;
    private AudioSource paintOn;

    public AudioClip paintSound;
    public AudioClip stopSound; 

    public int miniSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solverColor;



    private void Start()
    {
        solverColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solverColor;
      
    }





    private void FixedUpdate()
    {
        // Set the balls speed when it should travel
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;

        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(solverColor);
            }
            i++; 
        }



        // 
       

        // Check if we have reached our destination
        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;

            }

        }

        if (isTraveling)
            return;
        if (Input.GetMouseButton(0))
        {
            PlaySoundOnMove();
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

           

            if (swipePosCurrentFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < miniSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                //up /Down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    // Go UP/DOWN
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);

                }

                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    // Go Left/right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }

            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
        PlaySoundOnMove();
        SplashSomePaint();
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }

    //play sound FX my Sound method
    private void PlaySoundOnMove()
    {
        paintOn = GetComponent<AudioSource>();

        if (isTraveling)
        {
            paintOn.PlayOneShot(paintSound, 0.09f);
            
        }
    }

    //splash particle effect
    private void SplashSomePaint()
    {
       


        if (isTraveling) 
        {
            paintSplash.Play();
        }
        else
        {
            paintSplash.Stop();
        }
    }
}
