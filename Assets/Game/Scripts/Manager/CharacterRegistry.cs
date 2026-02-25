using System.Collections.Generic;
using UnityEngine;


public class CharacterRegistry : MonoBehaviour
{
    public static CharacterRegistry Instance { get; private set; }

   
    private readonly List<Character> _characters = new List<Character>(16);
    private readonly List<Transform> _coins      = new List<Transform>(64);
    private readonly List<Transform> _healths    = new List<Transform>(16);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    // ── Characters ────────────────────────────────────────────
    public static void Register(Character c)   { Instance?._characters.Add(c); }
    public static void Unregister(Character c) { Instance?._characters.Remove(c); }
    public IReadOnlyList<Character> Characters  => _characters;

    // ── Coins ─────────────────────────────────────────────────
    public static void RegisterCoin(Transform t)   { Instance?._coins.Add(t); }
    public static void UnregisterCoin(Transform t) { Instance?._coins.Remove(t); }
    public IReadOnlyList<Transform> Coins           => _coins;

    // ── Health ────────────────────────────────────────────────
    public static void RegisterHealth(Transform t)   { Instance?._healths.Add(t); }
    public static void UnregisterHealth(Transform t) { Instance?._healths.Remove(t); }
    public IReadOnlyList<Transform> Healths           => _healths;
}