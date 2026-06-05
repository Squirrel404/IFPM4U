using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController controller;
    [SerializeField] Transform camHolder;

    [Header("Position")]
    [SerializeField] Vector3 offset;
    [SerializeField] float amplitude = 0.015f;
    [SerializeField] Vector2 amplitudeHV = new Vector2(1, 1);

    [SerializeField] AnimationCurve horizontal;
    [SerializeField] AnimationCurve vertical;

    [Header("Rotation")]
    [SerializeField] float rotationAmount = 8;
    [SerializeField] float rotationSmooth = 99;

    [Header("Return")]
    [SerializeField] float returnSpeed = 3;

    float returnT;
    Vector3 returnStartPos;

    Quaternion targetRot;

    void Update()
    {
        if (!controller.Moving)
        {
            returnT += Time.deltaTime * returnSpeed;

            transform.localPosition = Vector3.Lerp(returnStartPos, offset, returnT);
            camHolder.localRotation = Quaternion.Slerp(targetRot, Quaternion.identity, returnT);

            return;
        }

        returnT = 0;

        Bob();

        returnStartPos = transform.localPosition;
    }

    void Bob()
    {
        float t = controller.walkPhase;
        float mult = amplitude * (controller.currentMoveSpeed / controller.walkSpeed);

        float x = horizontal.Evaluate(t) * mult * (controller.leftStep ? -1 : 1) * amplitudeHV.x;
        float y = vertical.Evaluate(t) * mult * amplitudeHV.y;

        transform.localPosition = new Vector3(0, y) + Camera.main.transform.right * x + offset;

        float yRot = -x * rotationAmount;
        float xRot = y * rotationAmount;
        float zRot = -x * rotationAmount * 0.5f;

        targetRot = Quaternion.Euler(xRot, yRot, zRot);

        camHolder.localRotation = Quaternion.Slerp(camHolder.localRotation, targetRot, Time.deltaTime * rotationSmooth);
    }
}