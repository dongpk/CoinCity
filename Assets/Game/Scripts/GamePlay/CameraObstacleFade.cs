using UnityEngine;

public class CameraObstacleFade : MonoBehaviour
{
    [Header("Cài đặt")]
    public Transform player;             // Kéo nhân vật vào đây
    public LayerMask obstacleLayer;      // Chọn Layer của tòa nhà (ví dụ "Building")

    [Header("Tinh chỉnh độ mờ")]
    [Range(0f, 1f)]
    public float fadedValue = 0.2f;      // Giá trị khi bị che (0 = mất hẳn, 0.2 = còn lấm chấm)
    public float originalValue = 1f;     // Giá trị khi hiện rõ (thường là 1)
    public float fadeSpeed = 5f;         // Tốc độ chuyển đổi (càng cao càng nhanh)

    // Tên biến trong Shader Graph (Quan trọng!)
    // Hãy đổi tên này nếu trong Shader Graph bạn đặt khác (ví dụ "_Float", "_Alpha", v.v.)
    private string shaderName = "_FadeValue";

    private Renderer currentRenderer;   // Lưu tòa nhà đang bị làm mờ

    void Update()
    {
        if (player == null) return;
        UpdateShader();
    }

    private void UpdateShader()
    {
        // Tính hướng từ Camera đến Player
        Vector3 dir = player.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.position);

        // Bắn tia Raycast
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dist, obstacleLayer))
        {
            //Debug.Log("Hit: " + hit.collider.name);
            Renderer hitRenderer = hit.collider.GetComponent<Renderer>();

            if (hitRenderer != null)
            {
                // Nếu va phải tòa nhà MỚI, thì trả tòa nhà CŨ về trạng thái bình thường ngay
                if (currentRenderer != null && currentRenderer != hitRenderer)
                {
                    ChangeFadeValue(currentRenderer, originalValue);
                }

                // Cập nhật tòa nhà hiện tại và làm mờ nó
                currentRenderer = hitRenderer;
                ChangeFadeValue(currentRenderer, fadedValue);
            }
        }
        else
        {
            // Nếu không bị che chắn gì cả, trả tòa nhà cũ về bình thường
            if (currentRenderer != null)
            {
                ChangeFadeValue(currentRenderer, originalValue);

                // Khi đã hiện rõ hoàn toàn (gần bằng 1), ta bỏ theo dõi nó
                // (Đoạn này kiểm tra đơn giản để tối ưu)
                float currentVal = currentRenderer.material.GetFloat(shaderName);
                if (Mathf.Abs(currentVal - originalValue) < 0.01f)
                {
                    currentRenderer = null;
                }
            }
        }
    }

    // Hàm thay đổi giá trị Shader (Lerp để mượt mà)
    void ChangeFadeValue(Renderer rend, float targetValue)
    {
        Material mat = rend.material;

        // Lấy giá trị hiện tại của biến _FadeValue
        float currentVal = mat.GetFloat(shaderName);

        // Tính toán giá trị mới (chuyển dần dần từ hiện tại sang target)
        float newVal = Mathf.Lerp(currentVal, targetValue, Time.deltaTime * fadeSpeed);

        // Gán ngược lại vào Material
        mat.SetFloat(shaderName, newVal);
    }
}