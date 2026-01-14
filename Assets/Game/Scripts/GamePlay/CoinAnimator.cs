using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    [SerializeField] float argularSpeed = 50f;// tốc độ quay quanh trục Y
    [SerializeField] float coinHeight = 1f; // chiều cao ban đầu của đồng xu
    [SerializeField] float movementAmplitude = .5f; // biên độ dao động
    [SerializeField] float movementFrequency = 1f; // tần số dao động
    [SerializeField] Transform coinMesh; // tham chiếu đến mesh của đồng xu

    private void Update()
    {
        coinMesh.Rotate(0f, argularSpeed * Time.deltaTime, 0f);

        // Tính toán vị trí Y mới dựa trên dao động hình sin
        float deltaY = movementAmplitude * Mathf.Sin(Time.time * movementFrequency ); 

        coinMesh.localPosition = new Vector3(0f, coinHeight + deltaY, 0f);
    }


}
