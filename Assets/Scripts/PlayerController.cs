using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb2d;

    private Animator myAnimator;
    public float speed;
    public float jumpForce;
    public LayerMask groundLayer;
    public Text countText;
    public Text scoreText;
    public Text livesText;
    public Text winText;
    public GameObject player;
    public AudioSource _audioSource;
    public AudioClip winSound;
    public float winVolume = 1.0f;

    private string nextLevel;
    private bool facingRight;
    private int count;
    private int score;
    private int lives;


    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        count = 0;
        score = 0;
        lives = 3;
        SetCountText();
        SetScoreText();
        SetLivesText();
        winText.text = "";
        facingRight = true;
        myAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        HandleMovement(horizontal);

        Flip(horizontal);
    }

    private void HandleMovement(float horizontal)
    {
        rb2d.velocity = new Vector2(horizontal * speed, rb2d.velocity.y);

        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            myAnimator.SetBool("inAir", false);
        }
      
        if (!IsGrounded())
        {
            return;
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                myAnimator.SetBool("inAir", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            score++;
            SetScoreText();
        }
        if (other.gameObject.CompareTag("Bad Pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            lives--;
            SetLivesText();
        }
    }


    private void GoToSecondLevel()
    {
        SceneManager.LoadScene("Second Level", LoadSceneMode.Single);
    }

    private void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    private void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        if (score == 4)
        {
            winText.text = "You Win!";

            _audioSource.PlayOneShot(winSound, winVolume);

        
            Scene m_Scene = SceneManager.GetActiveScene();

           
            string sceneName = m_Scene.name;

            
            if (sceneName == "First Level")
            {
            GoToSecondLevel();
            }
        }
    }

    private void SetLivesText()
    {
        livesText.text = "Lives: " + lives.ToString();

        if (lives == 0)
        {
            //Destorying the player throws errors, so it's better to Deactivate
            player.SetActive(false);

            //We'll reuse the win text because it's got the right size/location
            winText.text = "YOU DIED";
        }
    }

    bool IsGrounded()
    {
        
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 2.0f;
        //Debug.DrawRay(position, direction, Color.green);    
            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private void Flip(float horizontal)
    {
        if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;

            scale.x *= -1;

            transform.localScale = scale;
        }
    }
}
