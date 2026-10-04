using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorldClock.Maui;

public class ClockDisplayItem : INotifyPropertyChanged
{
    public string City { get; set; } = string.Empty;

    private string time = string.Empty;

    public string Time
    {
        get => time;

        set
        {
            if (time != value)
            {
                time = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}