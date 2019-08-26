using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball")
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g,sr.color.b, 0.1f);
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}
