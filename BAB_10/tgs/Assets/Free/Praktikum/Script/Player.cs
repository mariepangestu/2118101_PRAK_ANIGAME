using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public Animator animator;
  public int nyawa;
  [SerializeField] Vector3 respawn_loc;
  public bool play_again;
  Rigidbody2D rb;
  [SerializeField] Transform groundcheckCollider;
  [SerializeField] LayerMask groundLayer;

  const float groundCheckRadius = 0.2f; // +

  [SerializeField] float speed = 1;
    [SerializeField] float jumpPower = 100;

  float horizontalValue;
   [SerializeField] bool isGrounded; // +
  bool facingRight;
  bool jump;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    respawn_loc = transform.position;
  }

  void playagain()
  {
    nyawa = 3;
    transform.position = respawn_loc;
    play_again = false;
  }
  void Update ()
  {
    horizontalValue = Input.GetAxisRaw("Horizontal");
    if (Input.GetButtonDown("Jump"))
    {
    animator.SetBool("Jumping", true);
    jump = true;
    }
    else if (Input.GetButtonUp("Jump"))
    jump = false;

    if (nyawa<0)
    {
      playagain();
    }

    if (transform.position.y < -10)
    {
      play_again = true;
      playagain();
    }
  }

  void FixedUpdate()
  {
    GroundCheck();
    Move(horizontalValue, jump);
    animator.SetFloat("Blend", Mathf.Abs(rb.velocity.x));
    animator.SetFloat("Blend Jump", rb.velocity.y);
  }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fruit"))
        {
            // Mengaktifkan trigger 'catch' pada animator
            animator.SetTrigger("Catch");
            // Mengatur boolean 'Ready' menjadi true
            animator.SetBool("Ready", true);

            // Memulai coroutine untuk menghancurkan buah dan menonaktifkan boolean 'Ready' setelah animasi ancang-ancang
            StartCoroutine(CatchFruit(other.gameObject));
            
        }
    }

    IEnumerator CatchFruit(GameObject fruit)
    {
        // Menunggu sebentar untuk memberikan waktu bagi animasi ancang-ancang
        yield return new WaitForSeconds(0.2f);
        // Menghancurkan objek buah
        Destroy(fruit);

        // Menonaktifkan boolean 'Ready' setelah animasi ancang-ancang
        animator.SetBool("Ready", false);
        
    }

  void GroundCheck()
  {
    isGrounded = false;
    Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheckCollider.position, groundCheckRadius, groundLayer);
    if (colliders.Length > 0)
    {
    isGrounded = true;
    }
    animator.SetBool("Jumping", !isGrounded);
  }
  void Move(float dir, bool jumpflag)
  {
    if(isGrounded && jumpflag)
    {
        isGrounded = false;
        jumpflag = false;
        rb.AddForce(new Vector2(0f, jumpPower));
    }
    #region gerak kanan kiri
    float xVal = dir * speed * 100 * Time.fixedDeltaTime;
    Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
    rb.velocity = targetVelocity;

    if (facingRight && dir < 0)
    {
      // ukuran player
      transform.localScale = new Vector3(-1, 1, 1);
      facingRight = false;
    }

    else if (!facingRight && dir > 0)
    {
      // ukuran player
      transform.localScale = new Vector3(1, 1, 1);
      facingRight = true;
    }

    #endregion
  }
}
