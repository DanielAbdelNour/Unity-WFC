using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

namespace WFC.Editor
{
    public class WFCTools : EditorWindow
    {
        // private WFCGenerator _generatorInstance;

        
        [MenuItem("Window/WFC Tools")]
        public static void ShowWFCTools()
        {
            WFCTools wnd = GetWindow<WFCTools>();
            wnd.titleContent = new GUIContent("WFC Tools");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            var moduleSet = new ObjectField("Module Set") { allowSceneObjects = true, objectType = typeof(WFCModuleSet) };
            var moduleGameObjects = new ObjectField("Template") { allowSceneObjects = true, objectType = typeof(GameObject) };
            var cellGameObjects = new ObjectField("Cell GameObjects") { allowSceneObjects = true, objectType = typeof(GameObject) };
            var wfcGenerator = new ObjectField("WFC Generator") { allowSceneObjects = true, objectType = typeof(WFCGenerator) };
            var runWFC = new Button(){text="Run WFC"};
            var progressBar = new ProgressBar()
            {
                lowValue = 0,
                highValue = 100
            };
            var gridSize = new IntegerField()
            {
                value = 5,
                label = "Grid Size"
            };
            var generateModules = new Button(){text="Generate Module Assets", tooltip = "Generate an asset for each child GameObject in the collection"};
            var iterateWFC = new Button(){text="Iterate WFC"};
            var restart = new Button() { text = "Restart" };
            
            wfcGenerator.SetEnabled(false);
            cellGameObjects.SetEnabled(false);
            moduleSet.SetEnabled(false);
            iterateWFC.SetEnabled(false);
            runWFC.SetEnabled(false);
            

            moduleGameObjects.RegisterValueChangedCallback(evt => { generateModules.SetEnabled(evt.newValue != null); });
            generateModules.SetEnabled(false);
            
            restart.clicked += () =>
            {
                DestroyImmediate(GameObject.Find("WFCCells"));
                DestroyImmediate(GameObject.Find("GeneratedLevel"));
            };
            generateModules.clicked += () =>
            {
                WFCUtils.GenerateWFCModuleAssets((GameObject)moduleGameObjects.value);
                moduleSet.value = AssetDatabase.LoadAssetAtPath<WFCModuleSet>("Assets/WFC/Modules/ModuleSet.asset");
                WFCUtils.GenerateCells((WFCModuleSet)moduleSet.value, gridSize.value);
                cellGameObjects.value = GameObject.Find("WFCCells");
                var generatorInstance = CreateInstance<WFCGenerator>();
                generatorInstance.moduleSet = (WFCModuleSet)moduleSet.value;
                var cells = cellGameObjects.value.GetComponentsInChildren<WFCCell>().ToList();
                generatorInstance.cells = cells;
                AssetDatabase.CreateAsset(generatorInstance, "Assets/WFC/Modules/WFCGenerator.Asset");
                wfcGenerator.value = generatorInstance;
            };
            iterateWFC.clicked += () =>
            {
                ((WFCGenerator)wfcGenerator.value).Iterate();
            };
            runWFC.clicked += () =>
            {
                ((WFCGenerator)wfcGenerator.value).Generate();
            };
            
            root.Add(restart);
            root.Add(new Label("Module GameObjects from Scene") { style = { marginTop = 10, fontSize = 16, marginBottom = 7, marginLeft = 5 } });
            root.Add(moduleGameObjects);
            root.Add(gridSize);
            root.Add(generateModules);
            root.Add(moduleSet);
            root.Add(cellGameObjects);
            root.Add(wfcGenerator);
            root.Add(iterateWFC);
            root.Add(runWFC);
            root.Add(progressBar);

            wfcGenerator.RegisterValueChangedCallback(evt => iterateWFC.SetEnabled(evt.newValue != null && cellGameObjects.value != null));
            wfcGenerator.RegisterValueChangedCallback(evt => runWFC.SetEnabled(wfcGenerator.value != null));
            
            // DEBUGGING
            root.Add(new Label("Debugging") { style = { marginTop = 10, fontSize = 16, marginBottom = 7, marginLeft = 5 } });
            var moduleA = new ObjectField("Module A") { allowSceneObjects = true, objectType = typeof(WFCModule) };
            var direction = new EnumField("Direction", WFCUtils.Direction.Right);
            var validNeighbours = new ScrollView();
            root.Add(direction);
            root.Add(moduleA);
            // root.Add(moduleB);
            var compareModules = new Button() { text = "Get Valid Neighbours" };
            compareModules.clicked += () =>
            {
                validNeighbours.Clear();
                var a = WFCUtils.GetValidNeighboursForDirection(((WFCModuleSet)moduleSet.value).modules, (WFCModule)moduleA.value, (WFCUtils.Direction)direction.value);
                foreach (var mod in a)
                {
                    var obj = new TextElement()
                    {
                        text = mod.name
                    };
                    validNeighbours.Add(obj);
                }
            };
            root.Add(compareModules);
            root.Add(validNeighbours);
            // END DEBUGGING
        }
    }
}