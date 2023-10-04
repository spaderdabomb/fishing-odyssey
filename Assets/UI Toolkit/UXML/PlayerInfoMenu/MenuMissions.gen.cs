// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class MenuMissions
{
    private VisualElement menuMissionsRoot;
    private VisualElement missionsLeftContainer;
    private VisualElement missionTypeHeader;
    private QuickEye.UIToolkit.TabGroup missionTypeTabContainer;
    private Label currentMissionsHeaderLabel;
    private QuickEye.UIToolkit.TabDropdown missionsGroupBox;
    private VisualElement missionsRightContainer;
    private VisualElement currentMissionHeader;
    private Label missionHeaderNameLabel;
    private VisualElement currentMissionContainer;
    private VisualElement locationContainer;
    private Label currentMissionsLocationLabel;
    private Label questDescriptionLabel;
    private VisualElement missionStepContainer;
    private Label questStepLabel;
    private Button questStepNavButton;
    private VisualElement progressLayout;
    private VisualElement progressContainer;
    private Label progressLabel;
    private VisualElement progressBarLayout;
    private VisualElement progressBarContainer;
    private VisualElement progressBar;
    private Label missionStepProgressLabel;
    private VisualElement currentMissionRewardsContainer;
    private VisualElement currentMissionRewardsHeader;
    private VisualElement rewardsSlotContainer;
    private TemplateContainer ghostMissionContainer;
    
    protected void AssignQueryResults(VisualElement root)
    {
        menuMissionsRoot = root.Q<VisualElement>("MenuMissionsRoot");
        missionsLeftContainer = root.Q<VisualElement>("MissionsLeftContainer");
        missionTypeHeader = root.Q<VisualElement>("MissionTypeHeader");
        missionTypeTabContainer = root.Q<QuickEye.UIToolkit.TabGroup>("MissionTypeTabContainer");
        currentMissionsHeaderLabel = root.Q<Label>("CurrentMissionsHeaderLabel");
        missionsGroupBox = root.Q<QuickEye.UIToolkit.TabDropdown>("MissionsGroupBox");
        missionsRightContainer = root.Q<VisualElement>("MissionsRightContainer");
        currentMissionHeader = root.Q<VisualElement>("CurrentMissionHeader");
        missionHeaderNameLabel = root.Q<Label>("MissionHeaderNameLabel");
        currentMissionContainer = root.Q<VisualElement>("CurrentMissionContainer");
        locationContainer = root.Q<VisualElement>("LocationContainer");
        currentMissionsLocationLabel = root.Q<Label>("CurrentMissionsLocationLabel");
        questDescriptionLabel = root.Q<Label>("QuestDescriptionLabel");
        missionStepContainer = root.Q<VisualElement>("MissionStepContainer");
        questStepLabel = root.Q<Label>("QuestStepLabel");
        questStepNavButton = root.Q<Button>("QuestStepNavButton");
        progressLayout = root.Q<VisualElement>("ProgressLayout");
        progressContainer = root.Q<VisualElement>("ProgressContainer");
        progressLabel = root.Q<Label>("ProgressLabel");
        progressBarLayout = root.Q<VisualElement>("ProgressBarLayout");
        progressBarContainer = root.Q<VisualElement>("ProgressBarContainer");
        progressBar = root.Q<VisualElement>("ProgressBar");
        missionStepProgressLabel = root.Q<Label>("MissionStepProgressLabel");
        currentMissionRewardsContainer = root.Q<VisualElement>("CurrentMissionRewardsContainer");
        currentMissionRewardsHeader = root.Q<VisualElement>("CurrentMissionRewardsHeader");
        rewardsSlotContainer = root.Q<VisualElement>("RewardsSlotContainer");
        ghostMissionContainer = root.Q<TemplateContainer>("GhostMissionContainer");
    }
}
