using NaughtyAttributes;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController controller;
    [SerializeField] Transform camHolder;

    [Header("Position")]
    [SerializeField] Vector3 offset;
    [ShowNonSerializedField] Vector3 startLocalPositionOffset;
    Vector3 Offset { get { return offset + startLocalPositionOffset; } }
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

    void Awake()
    {
        startLocalPositionOffset = transform.localPosition;
    }

    void Update()
    {
        if (!controller.Moving)
        {
            returnT += Time.deltaTime * returnSpeed;

            transform.localPosition = Vector3.Lerp(returnStartPos, Offset, returnT);
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

        transform.localPosition = new Vector3(0, y) + Camera.main.transform.right * x + Offset;

        float yRot = -x * rotationAmount;
        float xRot = y * rotationAmount;
        float zRot = -x * rotationAmount * 0.5f;

        targetRot = Quaternion.Euler(xRot, yRot, zRot);

        camHolder.localRotation = Quaternion.Slerp(camHolder.localRotation, targetRot, Time.deltaTime * rotationSmooth);
    }

#if UNITY_EDITOR
    void Reset()
    {
        controller = transform.parent.GetComponent<PlayerController>();
        camHolder = GameObject.Find("Camera Holder").transform;
        startLocalPositionOffset = transform.localPosition;
        SetCurvesToDefault();
    }

    void SetCurvesToDefault()
    {
        Keyframe[] keys = new Keyframe[3]
        {
            new Keyframe(0, 0),
            new Keyframe(0.5f, 1),
            new Keyframe(1, 0)
        };
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].weightedMode = WeightedMode.None;
        }
        horizontal = new AnimationCurve(keys);
        vertical = new AnimationCurve(keys);
    }
#endif
}
