using System.ComponentModel;
using MvvmMonitor.Core.Mvvm;

namespace MvvmMonitor.Tests.ViewModels;

public sealed class ViewModelBaseTests
{
    private sealed class TestViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private int _age;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public int Age { get => _age; set => SetProperty(ref _age, value); }
    }

    [Fact]
    public void SetProperty_WhenValueChanges_RaisesPropertyChanged()
    {
        var vm = new TestViewModel();
        var events = new List<string>();
        vm.PropertyChanged += (_, e) => events.Add(e.PropertyName!);

        vm.Name = "Test";

        Assert.Contains("Name", events);
        Assert.Equal("Test", vm.Name);
    }

    [Fact]
    public void SetProperty_WhenValueDoesNotChange_DoesNotRaisePropertyChanged()
    {
        var vm = new TestViewModel { Name = "Test" };
        var events = new List<string>();
        vm.PropertyChanged += (_, e) => events.Add(e.PropertyName!);

        vm.Name = "Test";

        Assert.Empty(events);
    }

    [Fact]
    public void SetProperty_WithDifferentValues_RaisesForBoth()
    {
        var vm = new TestViewModel();
        var events = new List<string>();
        vm.PropertyChanged += (_, e) => events.Add(e.PropertyName!);

        vm.Name = "A";
        vm.Age = 25;

        Assert.Contains("Name", events);
        Assert.Contains("Age", events);
    }
}
