using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    //Animator
    private Animator _animator;

    void Start()
    {
        //Get animator component
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Basic Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        //Calculate the magnitude of the input
        float magnitude = direction.magnitude;

        //Set the parameters in the animator
        _animator.SetFloat("Speed", magnitude);
        _animator.SetFloat("Horizontal", horizontalInput);
        _animator.SetFloat("Vertical", verticalInput);
        
    }
}
