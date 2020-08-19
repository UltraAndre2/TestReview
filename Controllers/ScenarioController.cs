using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FemGame;
using System.Linq;

public class ScenarioController : MonoBehaviour
{
    private static Scenario _activeScenario = null;
    public static Scenario ActiveScenario {
        get {
            if (_activeScenario != null)
                return _activeScenario;
            else {
                Load();
                return ActiveScenario;
            }
        }
    }

    public static Scene GetParentByScene(Scene scene) {
        Scene result = null;

        foreach (Scene obj in ActiveScenario.scenario) {
            if (obj.nextScene == scene.nodeID ||
                (obj.HasBlock<Choice>() && obj.GetBlocks<Choice>().First().options.FindAll(x => x.@event == scene.nodeID).Count > 0) ||
                (obj.HasBlock<MiniGame>() && obj.GetBlocks<MiniGame>().First().winEvent == scene.nodeID))
                return obj;
        }

        return result;
    }

    public static bool HasParentByScene(Scene scene) {
        return GetParentByScene(scene) != null;
    }

    public static void Load() {
        _activeScenario = ScenarioParser.Parse(Resources.Load<TextAsset>("Scenario/Scenario"));
    }
}
