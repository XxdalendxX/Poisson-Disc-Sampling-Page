using UnityEngine;

namespace PoissonDiscSampling
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] Transform playerBody;
        [SerializeField] float mouseSensitivity = 1000f;
        [SerializeField] float speed = 0.5f;
    
        float xRotation = 0f;
        float yRotation = 0f;
        
        
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
    
            CameraLook();
            CameraMove();
        }
    
        void CameraLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
    
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    
            yRotation += mouseX;
    
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    
        void CameraMove()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 1f;
            }
            else
            {
                speed = 0.5f;
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                playerBody.Translate(Vector3.forward * speed);
            }
    
            if (Input.GetKey(KeyCode.S))
            {
                playerBody.Translate(-Vector3.forward * speed);
            }
    
            if (Input.GetKey(KeyCode.D))
            {
                playerBody.Translate(Vector3.right * speed);
            }
    
            if (Input.GetKey(KeyCode.A))
            {
                playerBody.Translate(-Vector3.right * speed);
            }
    
            if (Input.GetKey(KeyCode.Space))
            {
                playerBody.Translate(Vector3.up * speed);
            }
    
            if (Input.GetKey(KeyCode.LeftControl))
            {
                playerBody.Translate(-Vector3.up * speed);
            }
        }
    }
}
