# MetroWindow
A .NET6 DatePicker featuring auto-sizing, highlighted dates, simplified input and red underlined false input.

## Installation
1. Get [the package](https://www.nuget.org/packages/Laila.DatePicker/) from NuGet.

2. Add the files to your `<Resources />`:
```XML
<Application x:Class="Application"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/Laila.DatePicker;component/Themes/Blue.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

3. Put the control on your WPF form and bind the SelectedDate property and optionally the HighlightedDates property.
