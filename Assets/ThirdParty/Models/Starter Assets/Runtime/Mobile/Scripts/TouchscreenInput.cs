using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TouchscreenInput : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Move joystick magnitude is in [-1;1] range, this multiply it before sending it to move event")]
    public float MoveMagnitudeMultiplier = 1.0f;
    [Tooltip("Look joystick magnitude is in [-1;1] range, this multiply it before sending it to move event")]
    public float LookMagnitudeMultiplier = 1.0f;
    public bool InvertLookY;

    [Header("Events")]
    public UnityEvent<Vector2> MoveEvent;
    public UnityEvent<Vector2> LookEvent;
    public UnityEvent<bool> JumpEvent;
    public UnityEvent<bool> SprintEvent;

    private UIDocument m_Document;

    private VirtualJoystick m_MoveJoystick;
    private VirtualJoystick m_LookJoystick;

    private void Awake()
    {
        m_Document = GetComponent<UIDocument>();
    }

    // SỬA Ở ĐÂY: Đổi từ Start() thành OnEnable()
    private void OnEnable()
    {
        if (m_Document == null) return;

        // 1. Cập nhật lại Safe Area mỗi khi bật lên (đề phòng xoay màn hình khi đang tắt)
        var safeArea = Screen.safeArea;
        var root = m_Document.rootVisualElement;

        // Kiểm tra root null để tránh lỗi
        if (root == null) return;

        root.style.position = Position.Absolute;
        root.style.left = safeArea.xMin;
        root.style.right = Screen.width - safeArea.xMax;
        root.style.top = Screen.height - safeArea.yMax;
        root.style.bottom = safeArea.yMin;

        // 2. Tìm lại các Element mới (vì Element cũ đã mất khi SetActive false)
        var joystickMove = root.Q<VisualElement>("JoystickMove");
        var joystickLook = root.Q<VisualElement>("JoystickLook");

        // Gán lại logic Joystick Move
        m_MoveJoystick = new VirtualJoystick(joystickMove);
        m_MoveJoystick.JoystickEvent.AddListener(mov =>
        {
            MoveEvent.Invoke(mov * MoveMagnitudeMultiplier);
        });

        // Gán lại logic Joystick Look
        m_LookJoystick = new VirtualJoystick(joystickLook);
        m_LookJoystick.JoystickEvent.AddListener(mov =>
        {
            if (InvertLookY)
                mov.y *= -1;

            LookEvent.Invoke(mov * LookMagnitudeMultiplier);
        });

        // Gán lại nút Jump
        var jumpButton = root.Q<VisualElement>("ButtonJump");
        // Unregister trước để tránh duplicate (dù new VirtualJoystick đã tạo mới nhưng cẩn thận vẫn hơn)
        // Nhưng với UI Toolkit, khi element được tạo lại thì callback cũ tự mất, nên đăng ký mới là ok.
        if (jumpButton != null)
        {
            jumpButton.RegisterCallback<PointerEnterEvent>(evt => { JumpEvent.Invoke(true); });
            jumpButton.RegisterCallback<PointerLeaveEvent>(evt => { JumpEvent.Invoke(false); });
        }

        // Gán lại nút Sprint
        var sprintButton = root.Q<VisualElement>("ButtonSprint");
        if (sprintButton != null)
        {
            sprintButton.RegisterCallback<PointerEnterEvent>(evt => { SprintEvent.Invoke(true); });
            sprintButton.RegisterCallback<PointerLeaveEvent>(evt => { SprintEvent.Invoke(false); });
        }
    }
}

// Class VirtualJoystick giữ nguyên, không cần sửa
public class VirtualJoystick
{
    public VisualElement BaseElement;
    public VisualElement Thumbstick;

    public UnityEvent<Vector2> JoystickEvent = new();

    public VirtualJoystick(VisualElement root)
    {
        // Thêm kiểm tra null để tránh lỗi nếu UI chưa load kịp
        if (root == null) return;

        BaseElement = root;
        Thumbstick = root.Q<VisualElement>("JoystickHandle");

        BaseElement.RegisterCallback<PointerDownEvent>(HandlePress);
        BaseElement.RegisterCallback<PointerMoveEvent>(HandleDrag);
        BaseElement.RegisterCallback<PointerUpEvent>(HandleRelease);
    }
    // ... (Phần dưới giữ nguyên) ...
    void HandlePress(PointerDownEvent evt)
    {
        BaseElement.CapturePointer(evt.pointerId);
    }

    void HandleRelease(PointerUpEvent evt)
    {
        BaseElement.ReleasePointer(evt.pointerId);

        Thumbstick.style.left = Length.Percent(50);
        Thumbstick.style.top = Length.Percent(50);

        JoystickEvent.Invoke(Vector2.zero);
    }

    void HandleDrag(PointerMoveEvent evt)
    {
        if (!BaseElement.HasPointerCapture(evt.pointerId)) return;

        var width = BaseElement.contentRect.width;
        var center = new Vector3(width / 2, width / 2);
        var centerToPosition = evt.localPosition - center;

        if (centerToPosition.magnitude > width / 2)
        {
            centerToPosition = centerToPosition.normalized * width / 2;
        }

        var newPos = center + centerToPosition;

        Thumbstick.style.left = newPos.x;
        Thumbstick.style.top = newPos.y;

        centerToPosition /= (width / 2);
        centerToPosition.y *= -1;

        JoystickEvent.Invoke(centerToPosition);
    }
}