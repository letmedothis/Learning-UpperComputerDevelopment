using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmMonitor.Core.Mvvm;

/// <summary>
/// ViewModel 基类，实现 INotifyPropertyChanged。
/// Java 对比：类似 JavaFX 的 Observable，属性变化时自动通知 UI 更新。
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 设置属性值并在值变化时触发通知。
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
