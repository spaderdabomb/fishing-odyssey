// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class UIGameScene
{
    private VisualElement gameSceneContainer;
    private VisualElement rightUI;
    private VisualElement bottomRightUI;
    private VisualElement fishPowerContainer;
    private VisualElement fishPowerMeter;
    private VisualElement allMenus;
    private VisualElement menuDimBg;
    private VisualElement menuCollectionsContainer;
    private ScrollView scrollViewCollections;
    private VisualElement menuOptions;
    private VisualElement optionsHeaaderContainer;
    private VisualElement optionsButtonContainer;
    
    protected void AssignQueryResults(VisualElement root)
    {
        gameSceneContainer = root.Q<VisualElement>("GameSceneContainer");
        rightUI = root.Q<VisualElement>("RightUI");
        bottomRightUI = root.Q<VisualElement>("BottomRightUI");
        fishPowerContainer = root.Q<VisualElement>("FishPowerContainer");
        fishPowerMeter = root.Q<VisualElement>("FishPowerMeter");
        allMenus = root.Q<VisualElement>("AllMenus");
        menuDimBg = root.Q<VisualElement>("MenuDimBg");
        menuCollectionsContainer = root.Q<VisualElement>("MenuCollectionsContainer");
        scrollViewCollections = root.Q<ScrollView>("ScrollViewCollections");
        menuOptions = root.Q<VisualElement>("MenuOptions");
        optionsHeaaderContainer = root.Q<VisualElement>("OptionsHeaaderContainer");
        optionsButtonContainer = root.Q<VisualElement>("OptionsButtonContainer");
    }
}
