Imports System.ComponentModel

Class MainWindow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _selectedDate As DateTime?

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = Me
    End Sub
    Public Property SelectedDate As DateTime?
        Get
            Return _selectedDate
        End Get
        Set(value As DateTime?)
            _selectedDate = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("SelectedDate"))
        End Set
    End Property

    Public ReadOnly Property HighlightedDates As List(Of DateTime)
        Get
            Return New List(Of DateTime) From {
                DateTime.Now.Date,
                DateTime.Now.Date.AddDays(1),
                DateTime.Now.Date.AddDays(-1)
            }
        End Get
    End Property
End Class
