using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 5f;        // ความเร็วในการเลื่อนกล้อง
    public float zoomSpeed = 5f;       // ความเร็วในการซูม
    public float minZoom = 2f;         // ระยะซูมต่ำสุด
    public float maxZoom = 10f;        // ระยะซูมสูงสุด

    private bool isDragging = false;   // ตรวจสอบว่ากำลังกดเมาส์เพื่อเลื่อนอยู่หรือไม่
    private Vector3 initialMousePosition;

    void Update()
    {
        // ======== การเลื่อนกล้องด้วยปุ่ม A และ D ========
        float horizontalInput = Input.GetAxis("Horizontal"); // รับค่า A และ D
        transform.position += new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, 0);

        // ======== การเลื่อนกล้องด้วยการคลิกเมาส์ ========
        if (Input.GetMouseButtonDown(0)) // เมื่อกดเมาส์ซ้าย
        {
            isDragging = true;
            initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0)) // เมื่อปล่อยเมาส์ซ้าย
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float deltaX = currentMousePosition.x - initialMousePosition.x;

            transform.position += new Vector3(deltaX, 0, 0);
            initialMousePosition = currentMousePosition;
        }

        // ======== การซูมกล้อง ========
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // ตรวจสอบการหมุน Scroll Wheel
        Camera.main.orthographicSize -= scroll * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
    }
}