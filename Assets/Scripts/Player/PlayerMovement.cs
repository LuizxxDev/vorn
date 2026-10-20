using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private const int DirDown = 0;
    private const int DirLeft = 1;
    private const int DirRight = 2;
    private const int DirUp = 3;

    private static readonly int DirectionHash = Animator.StringToHash("Direction");

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D body;
    private Animator animator;
    private Vector2 moveInput;
    private int direction = DirDown;
    private bool wasMoving;

    public Vector2 FacingVector
    {
        get
        {
            switch (direction)
            {
                case DirLeft: return Vector2.left;
                case DirRight: return Vector2.right;
                case DirUp: return Vector2.up;
                default: return Vector2.down;
            }
        }
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Começa parado, no primeiro quadro de Walk_Down.
        animator.speed = 0f;
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = moveInput * moveSpeed;
    }

    private void ReadInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;

        // normalized garante que a diagonal tenha o mesmo comprimento (1) das outras direções
        moveInput = new Vector2(x, y).normalized;
    }

    private void UpdateAnimation()
    {
        bool isMoving = moveInput != Vector2.zero;

        if (isMoving)
        {
            direction = GetDirection(moveInput, direction);
            animator.SetInteger(DirectionHash, direction);
            animator.speed = 1f;
        }
        else if (wasMoving)
        {
            // Acabou de parar: volta o clip atual ao primeiro quadro e congela.
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            animator.speed = 0f;
        }

        wasMoving = isMoving;
    }

    private int GetDirection(Vector2 input, int currentDirection)
    {
        bool movingHorizontally = !Mathf.Approximately(input.x, 0f);
        bool movingVertically = !Mathf.Approximately(input.y, 0f);

        // Diagonal: mantém o eixo que o personagem já estava usando,
        // para a animação não ficar trocando de lado quando se aperta duas teclas.
        if (movingHorizontally && movingVertically)
        {
            bool currentIsHorizontal = currentDirection == DirLeft || currentDirection == DirRight;

            if (currentIsHorizontal)
            {
                return input.x < 0f ? DirLeft : DirRight;
            }

            return input.y < 0f ? DirDown : DirUp;
        }

        if (movingHorizontally)
        {
            return input.x < 0f ? DirLeft : DirRight;
        }

        return input.y < 0f ? DirDown : DirUp;
    }
}