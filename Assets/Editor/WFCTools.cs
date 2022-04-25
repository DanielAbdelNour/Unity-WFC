using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class WFCTools : EditorWindow
{
    [MenuItem("Window/WFC Tools")]
    public static void ShowWFCTools()
    {
        WFCTools wnd = GetWindow<WFCTools>();
        wnd.titleContent = new GUIContent("WFC Tools");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualElement generateModules = new Button();
        generateModules.Add(new Label("Generate Modules"));
        TextField numberOfGameobjectsStat = new TextField("Number of Gameobjects");
        TextField numberOfMeshFiltersStat = new TextField("Number of MeshFilters");
        TextField numberOfUniqueMeshesStat = new TextField("Number of Unique Meshes");

        generateModules.SetEnabled(false);

        numberOfGameobjectsStat.SetEnabled(false);
        numberOfMeshFiltersStat.SetEnabled(false);
        numberOfUniqueMeshesStat.SetEnabled(false);

        ObjectField moduleGameObjects = new ObjectField("Module Gameobjects") { allowSceneObjects = true, objectType = typeof(GameObject) };
        moduleGameObjects.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != null)
            {
                generateModules.SetEnabled(true);
                numberOfGameobjectsStat.value = (evt.newValue as GameObject).transform.childCount.ToString();
                numberOfMeshFiltersStat.value = (evt.newValue as GameObject).GetComponentsInChildren<MeshFilter>().Length.ToString();
                var uniqueMeshes = Utils.GetUniqueMeshes(evt.newValue as GameObject);
                numberOfUniqueMeshesStat.value = uniqueMeshes.Count.ToString();
            }
            else
            {
                generateModules.SetEnabled(false);
                numberOfGameobjectsStat.value = "No Gameobject selected";
                numberOfMeshFiltersStat.value = "No Gameobject selected";
                numberOfUniqueMeshesStat.value = "No Gameobject selected";
            }
        });

        root.Add(new Label("Module Gameobjects from Scene") { style = { marginTop = 10, fontSize = 16, marginBottom = 7, marginLeft = 5 } });

        root.Add(moduleGameObjects);
        root.Add(numberOfGameobjectsStat);
        root.Add(numberOfMeshFiltersStat);
        root.Add(numberOfUniqueMeshesStat);
        root.Add(generateModules);

        // DEBUGGING
        root.Add(new Label("Debugging") { style = { marginTop = 10, fontSize = 16, marginBottom = 7, marginLeft = 5 } });
        ObjectField moduleAField = new ObjectField("Module A") { allowSceneObjects = true, objectType = typeof(GameObject) };
        ObjectField moduleBField = new ObjectField("Module B") { allowSceneObjects = true, objectType = typeof(GameObject) };
        root.Add(moduleAField);
        root.Add(moduleBField);
        var compareModules = new Button() { text = "Compare Modules" };
        compareModules.clicked += () =>
        {
            Utils.GetMeshHash(moduleAField.value as GameObject);
            Utils.GetMeshHash(moduleBField.value as GameObject);
        };
        root.Add(compareModules);
        // END DEBUGGING

        root.Add(new Label("Iterate Steps") { style = { marginTop = 10, fontSize = 16, marginBottom = 7, marginLeft = 5 } });

        VisualElement generateCells = new Button();
        VisualElement generateCellsText = new Label("Generate Cells");
        generateCells.Add(generateCellsText);
        root.Add(generateCells);

        VisualElement iterateWFC = new Button();
        VisualElement iterateWFCText = new Label("Iterate WFC");
        iterateWFC.Add(iterateWFCText);
        root.Add(iterateWFC);

        root.Add(new Label("WFC Algorithm") { style = { marginTop = 10, fontSize = 16, marginBottom = 7, marginLeft = 5 } });

        VisualElement runWFC = new Button();
        VisualElement runWFCText = new Label("Run WFC");
        runWFC.Add(runWFCText);
        root.Add(runWFC);




        // root.Add(lineBreak);


        // // Each editor window contains a root VisualElement object
        // VisualElement root = rootVisualElement;

        // // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // VisualElement label = new Label("Hello World! From C#");
        // root.Add(label);

        // VisualElement button = new Button(() => { Debug.Log("Clicked"); });
        // button.Add(new Label("Click Me"));
        // root.Add(button);

        // ObjectField field = new ObjectField("add stuff") { allowSceneObjects = true, objectType = typeof(GameObject) };
        // root.Add(field);

        // field.RegisterCallback<ChangeEvent<Object>>(evt =>
        // {
        //     Debug.Log("Object changed to " + evt.newValue);
        //     Debug.Log((evt.newValue as GameObject).GetComponent<MeshFilter>());
        // });

        // // Import UXML
        // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/WFCTools.uxml");
        // VisualElement labelFromUXML = visualTree.Instantiate();
        // root.Add(labelFromUXML);

        // // A stylesheet can be added to a VisualElement.
        // // The style will be applied to the VisualElement and all of its children.
        // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/WFCTools.uss");
        // VisualElement labelWithStyle = new Label("Hello World! With Style");
        // labelWithStyle.styleSheets.Add(styleSheet);
        // root.Add(labelWithStyle);

        // VisualElement lineBreak = new VisualElement();
        // lineBreak.style.borderTopColor = Color.white;
        // lineBreak.style.borderTopWidth = 2;

        // root.Add(lineBreak);



    }
}