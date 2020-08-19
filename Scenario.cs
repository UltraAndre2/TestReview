using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace FemGame {

    public class ScenarioRoadMap
    {
        public ScenarioRoadPoint EntryPoint;
        public Scenario scenario;

        public ScenarioRoadMap(Scenario scenario)
        {
            this.EntryPoint = null;
            this.scenario = scenario;
        }

        public static ScenarioRoadMap Initialize(Scenario scenario)
        {
            ScenarioRoadMap scenarioRoadMap = new ScenarioRoadMap(scenario);

            scenario.scenario.ForEach(scene =>
            {
                if (scenarioRoadMap.EntryPoint == null)
                    scenarioRoadMap.EntryPoint = new ScenarioRoadPoint(scene.nodeID);
            });

            return scenarioRoadMap;
        }
    }

    public class ScenarioRoadPoint
    {
        private ScenarioRoadPoint parent = null;
        private ScenarioRoadPoint child = null;

        public string sceneID = "";

        public ScenarioRoadPoint(string sceneID)
        {
            this.sceneID = sceneID;
        }

        public ScenarioRoadPoint Child()
        {
            return HasChild() ? child : this;
        }

        public ScenarioRoadPoint Parent()
        {
            return HasParent() ? parent : this;
        }

        public bool HasChild() {
            return this.child != null;
        }

        public bool HasParent()
        {
            return this.parent != null;
        }

        public ScenarioRoadPoint SetChild(string sceneID)
        {
            this.child = new ScenarioRoadPoint(sceneID).SetParent(this);

            return this;
        }
        public ScenarioRoadPoint SetParent(ScenarioRoadPoint parent)
        {
            this.parent = parent;

            return this;
        }
    }

    public class Scenario
    {
        public List<Scene> scenario;
        public List<Person> persons;

        public Scenario()
        {
            this.persons = new List<Person>();
            this.scenario = new List<Scene>();
        }
    }

    public class Scene
    {
        public string type;
        public string nodeID;
        public string nextScene;
        public List<Block> blocks;

        public bool HasBlock<T>()
        {
            return GetBlocks<T>().Count > 0;
        }

        public List<T> GetBlocks<T>()
        {
            return this.blocks.FindAll(x => x is T).Cast<T>().ToList();
        }
    }

    public class Person
    {
        public string name;
        public int skin;
    }

    public class Block
    {
        public string type;

        public Block()
        {
            type = this.GetType().Name;
        }

        public virtual void Execute(Action<ActionEvent> onComplete)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BackgroundAction : Block
    {
        public string background;

        public BackgroundAction(string background) : base()
        {
            this.background = background;
        }

        public override void Execute(Action<ActionEvent> onComplete)
        {
            QuestHandler.GetInstance().ExecuteBlock(this, onComplete);
        }
    }

    public class PersonAction : Block
    {
        public const string ENTER = "Enter";
        public const string LEAVE = "Leave";

        public int target;
        public string action;
        public PersonEmotion emotion;

        public PersonAction(int target, string action, PersonEmotion emotion) : base()
        {
            this.target = target;
            this.action = action;
            this.emotion = emotion;
        }

        public override void Execute(Action<ActionEvent> onComplete)
        {
            QuestHandler.GetInstance().ExecuteBlock(this, onComplete);
        }
    }

    public class VoiceOver : Block
    {
        public string message;

        public VoiceOver(string message) : base()
        {
            this.message = message;
        }

        public override void Execute(Action<ActionEvent> onComplete)
        {
            QuestHandler.GetInstance().ExecuteBlock(this, onComplete);
        }
    }

    public class Message : Block
    {
        public string message;
        public int target;
        public PersonEmotion emotion;

        public Message(string message, int target, PersonEmotion emotion) : base()
        {
            this.message = message;
            this.target = target;
            this.emotion = emotion;
        }

        public override void Execute(Action<ActionEvent> onComplete)
        {
            QuestHandler.GetInstance().ExecuteBlock(this, onComplete);
        }
    }

    public class Choice : Block
    {
        public List<ChoiceOption> options;

        public Choice(List<ChoiceOption> options) : base()
        {
            this.options = options;
        }

        public override void Execute(Action<ActionEvent> onComplete)
        {
            QuestHandler.GetInstance().ExecuteBlock(this, onComplete);
        }
    }

    public class ChoiceOption : Block
    {
        public string message;
        public string @event;
        public int eventPrice;

        public ChoiceOption(string message, string @event, int eventPrice) : base()
        {
            this.message = message;
            this.@event = @event;
            this.eventPrice = eventPrice;
        }

        public override void Execute(Action<ActionEvent> onComplete)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PersonEmotion
    {
        public static PersonEmotion NEUTRAL = new PersonEmotion("NEUTRAL", "NEUTRAL");

        public string head;
        public string body;

        public PersonEmotion(string head, string body)
        {
            this.head = head;
            this.body = body;
        }
    }

    public class MiniGame : Block
    {
        public string itemCollection;
        public string message;
        public string winEvent;
        public int goal;

        public override void Execute(Action<ActionEvent> onComplete)
        {
            QuestHandler.GetInstance().ExecuteBlock(this, onComplete);
        }
    }


    public class ActionEvent
    {
        public string sceneEvent;

        public ActionEvent(string sceneEvent)
        {
            this.sceneEvent = sceneEvent;
        }

        public static bool operator ==(ActionEvent a, ActionEvent b)
            => a.sceneEvent == b.sceneEvent;
        public static bool operator !=(ActionEvent a, ActionEvent b)
            => a.sceneEvent != b.sceneEvent;

        public static ActionEvent NONE = new ActionEvent("NONE");
        public static ActionEvent FINAL = new ActionEvent("FINAL");
    }

    public class ScenarioPoint
    {
        public const string START_POINT = "c1s1";

        public string sceneID;
        public int blockID;

        public ScenarioPoint(string sceneID, int blockID)
        {
            this.sceneID = sceneID;
            this.blockID = blockID;
        }

        public static ScenarioPoint START
        {
            get {
                return new ScenarioPoint(START_POINT, 0);
            }
        }

        public static bool operator ==(ScenarioPoint a, ScenarioPoint b)
            => a.sceneID == b.sceneID && a.blockID == b.blockID;
        public static bool operator !=(ScenarioPoint a, ScenarioPoint b)
            => !(a.sceneID == b.sceneID && a.blockID == b.blockID);
    }

    public class ScenarioParser {
        public static Scenario Parse(TextAsset json) {
            Scenario scenario = JSON.From<Scenario>(json.text);
            JObject rawScenario = Newtonsoft.Json.Linq.JObject.Parse(json.text);

            foreach(JObject scene in (JArray)rawScenario["scenario"]) {
                int sceneID = ((JArray)rawScenario["scenario"]).IndexOf(scene);
                scenario.scenario[sceneID].blocks = new List<Block>();
                
                foreach (JObject sceneBlock in scene["blocks"]) {
                    ApplySceneBlock(sceneBlock["type"].ToString());
                }
            }

            return scenario;
        }

        public static void ApplySceneBlock(string sceneBlockType) {
            switch (sceneBlockType)
            {
                case "BackgroundAction":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<BackgroundAction>(sceneBlock.ToString()));
                    break;
                case "Music":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<Music>(sceneBlock.ToString()));
                    break;
                case "PersonAction":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<PersonAction>(sceneBlock.ToString()));
                    break;
                case "VoiceOver":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<VoiceOver>(sceneBlock.ToString()));
                    break;
                case "Message":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<Message>(sceneBlock.ToString()));
                    break;
                case "Choice":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<Choice>(sceneBlock.ToString()));
                    break;
                case "MiniGame":
                    scenario.scenario[sceneID].blocks.Add(JSON.From<MiniGame>(sceneBlock.ToString()));
                    break;
            }
        }
    }
}