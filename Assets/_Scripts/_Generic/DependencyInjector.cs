using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class DependencyInjector : MonoBehaviour, IDependencyInjector
{
    [TabGroup("tab", "DependencyInjector")] [SerializeField] private Tray _tray;
    [TabGroup("tab", "DependencyInjector")] [SerializeField] private Initializer _initializer;
    [TabGroup("tab", "DependencyInjector")] [SerializeField] private Score score;
    [TabGroup("tab", "DependencyInjector")] [SerializeField] private Settings settings;

    private void Awake()
    {
        InjectDependency();
    }

    public void InjectDependency()
    {
        _tray.Scorer = score;
        _initializer.TrayInterface = _tray;
        _initializer.Scorer = score;
        _initializer.SettingInterface = settings;
     
    }
}
