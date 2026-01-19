using UnityEngine;

[CreateAssetMenu(fileName = "BotConfig", menuName = "Game/BotConfig")]
public class BotConfig : ScriptableObject
{
    public string botName;
    public int maxHealth = 100;
    public float moveSpeed=5f;

    [Space(20)]
    [Header("Settings")]
    [Tooltip("độ hung hăng của bot, từ 0 đến 1")]
    [Range(0f,1f)] public float aggressiveness=0.5f;
    [Tooltip("tầm phát hiện")]
    public float dectecionRange = 10f;
    [Tooltip("khoảng cách tấn công")]
    public float attackRange = 1f;
    [Tooltip("% máu để bỏ chạy")]
    public float feelHealthThreshHold = .3f;

    [Space(20)]
    [Header("Skin")]
    [Tooltip("skin")]
    public int skinIndex = 0;

    [Space(20)]
    [Header("Combat")]
    [Tooltip("sát thương mỗi lần đánh")]
    public int attackDmg = 10;
    [Tooltip("thời gian mỗi lần đánh")]
    public float attackCooldown = 1f;
    [Tooltip("bán kính di chuyển/tuần tra")]
    public float patrolRadius = 10f;
}
