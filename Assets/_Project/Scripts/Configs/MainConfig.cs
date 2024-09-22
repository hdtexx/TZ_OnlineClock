using UnityEngine;

namespace _Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "MainConfig", menuName = "GameConfigs/MainConfig")]
    public class MainConfig : ScriptableObject
    {
        [field: Header("Time server settings:")]
        [field: SerializeField] public string TimeServerUrl { get; private set; } = "https://yandex.com/time/sync.json";
        [field: SerializeField] public string TimeKeyJSON { get; private set; } = "time";
        [field: SerializeField] public double ServerWaitingForResponseTime { get; private set; } = 5;
        [field: SerializeField] public float AutoSyncInterval { get; private set; } = 3600f;
        
        [field: Space(10)]
        [field: Header("Game settings:")]
        [field: SerializeField] public Vector2 ClockPosition { get; private set; } = new Vector2(0f, 0.5f);
    }
}