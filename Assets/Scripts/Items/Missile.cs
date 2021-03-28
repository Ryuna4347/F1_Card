using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Range(4,15)]public float missileSpeed;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    public List<AudioClip> sounds;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sounds[0];
        audioSource.Play();
    }

    private void Update()
    {
        rb.velocity = transform.up * missileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Car"))
        {
            rb.isKinematic = true;
            audioSource.clip = sounds[1];
            audioSource.Play();
            collision.gameObject.GetComponent<CarMovement>().StartCoroutine("GetDamaged");
            Destroy(this.gameObject, 0.2f);
        }
        else if (collision.gameObject.CompareTag("Border")) {
            Destroy(this.gameObject);
        }
    }
}
