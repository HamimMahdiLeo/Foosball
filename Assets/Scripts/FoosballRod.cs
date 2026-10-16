using UnityEngine;

public class FoosballRod : MonoBehaviour
{
    public float minSlide = -0.35f;
    public float maxSlide = 0.35f;

    public float minAngle = -90f;

    public float maxAngle = 90f;
    

    private Vector3 startPos;
    private Quaternion startRot;
    private float slideOffset;
    private float currentAngle;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    public void MoveAndRotate(float lsY, float rsX, float slideSpeed, float maxAngle, float snapSpeed)
    {
        // 1. Slide Up/Down (Left Stick Y)
        slideOffset = Mathf.Clamp(slideOffset + lsY * slideSpeed * Time.deltaTime, minSlide, maxSlide);
        transform.localPosition = startPos + new Vector3(slideOffset, 0f, 0f);

        // 2. Tilt & Hold Angle (Right Stick X), return to 0 when released
        currentAngle = Mathf.Lerp(currentAngle, rsX * maxAngle, snapSpeed * Time.deltaTime);
        transform.localRotation = startRot * Quaternion.Euler(currentAngle, 0f, 0f);
    }
}