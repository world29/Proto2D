﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D), typeof(AudioSource))]
public class ItemController : MonoBehaviour
{
    public ItemType itemType;

    public GameObject pickupEffectPrefab;
    public AudioClip pickupSound;

    AudioSource audioSource;

    void Start () {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject receiver = collision.gameObject;

            ExecuteEvents.Execute<IItemReceiver>(receiver, null,
                (target, eventTarget) => target.OnPickupItem(itemType, gameObject));

            OnPickedUp(receiver);
        }
    }

    void OnPickedUp(GameObject receiver)
    {
        if (pickupEffectPrefab)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, receiver.transform.position, Quaternion.identity, null);
        }

        float delayToDestroy = 0;
        if (pickupSound)
        {
            audioSource.PlayOneShot(pickupSound);
            delayToDestroy = pickupSound.length;
        }
        Destroy(gameObject, delayToDestroy);

        // 描画とコリジョンを無効化
        DisableComponent<SpriteRenderer>();
        DisableComponent<BoxCollider2D>();
    }

    private void DisableComponent<T>()
    {
        T component = GetComponent<T>();
        if (component != null)
        {
            // http://answers.unity.com/answers/417142/view.html

            if (component is Behaviour)
                (component as Behaviour).enabled = false;
            else if (component is Collider)
                (component as Collider).enabled = false;
            else if (component is Renderer)
                (component as Renderer).enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            Gizmos.color = new Color(1, 1, 0, .3f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }

}