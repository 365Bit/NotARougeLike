using UnityEngine;

public class OpponentDefinitions : MonoBehaviour
{
    public OpponentClassDefinition[] classes;

    void Awake() {
        // normalize probabilities
        float probabilitySum = 0.0f;
        foreach(OpponentClassDefinition def in classes)
            probabilitySum += def.spawnProbability;

        foreach(OpponentClassDefinition def in classes)
            def.spawnProbability /= probabilitySum;
    }
}

[System.Serializable]
public class OpponentClassDefinition {
    public string className;
    public float spawnProbability;
    public GameObject prefab;
    [Tooltip("Defines scaling of stats in the opponent level")]
    public KeyValueStore<OpponentStatKey, InterpolationScaling> stats;
};

