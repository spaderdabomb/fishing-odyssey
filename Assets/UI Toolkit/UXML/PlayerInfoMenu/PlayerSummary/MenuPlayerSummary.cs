using UnityEngine.UIElements;
using UnityEngine;

public partial class MenuPlayerSummary
{
    public VisualElement root;
    public MenuPlayerSummary(VisualElement root)
    {
        this.root = root;
        AssignQueryResults(root);
    }
}
