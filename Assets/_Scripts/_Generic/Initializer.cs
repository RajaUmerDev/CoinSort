using UnityEngine;

public class Initializer : MonoBehaviour
{
    public IScorer Scorer { get; set; }
    public ISetter SettingInterface { get; set; }
    public ITray TrayInterface { get; set; }

    private void Start()
    {
        TrayInterface.Initialize();
        Scorer.Initialize();
        SettingInterface.Initialize();
    }
    
}
