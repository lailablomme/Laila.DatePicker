Imports System.Globalization
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input

Public Class DatePicker
    Inherits System.Windows.Controls.DatePicker

    Protected _textBox As DatePickerTextBox
    Protected _calendar As System.Windows.Controls.Calendar

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(DatePicker), New FrameworkPropertyMetadata(GetType(DatePicker)))
    End Sub

    Public Sub New()
        MyBase.IsTabStop = False
    End Sub

    Public Shared Shadows ReadOnly SelectedDateProperty As DependencyProperty =
        DependencyProperty.Register("SelectedDate", GetType(DateTime?), GetType(DatePicker),
            New FrameworkPropertyMetadata(Nothing, AddressOf SelectedDateChanged) With {.BindsTwoWayByDefault = True})

    Private Shared ReadOnly IsInvalidPropertyKey As DependencyPropertyKey =
        DependencyProperty.RegisterReadOnly("IsInvalid", GetType(Boolean), GetType(DatePicker), New FrameworkPropertyMetadata(False))
    Public Shared ReadOnly IsInvalidProperty As DependencyProperty = IsInvalidPropertyKey.DependencyProperty

    Public Shared ReadOnly HighlightedDatesProperty As DependencyProperty = DependencyProperty.Register("HighlightedDates", GetType(List(Of DateTime)), GetType(DatePicker))

    Public Shared Shadows ReadOnly IsTabStopProperty As DependencyProperty = DependencyProperty.Register("IsTabStop", GetType(Boolean), GetType(DatePicker), New FrameworkPropertyMetadata(True))

    Private Shared Shadows Sub SelectedDateChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        CType(d, DatePicker).SelectedDate = e.NewValue
    End Sub

    Public Overloads Property SelectedDate As DateTime?
        Get
            Return GetValue(SelectedDateProperty)
        End Get
        Set(value As DateTime?)
            If MyBase.SelectedDate.HasValue <> value.HasValue OrElse (MyBase.SelectedDate.HasValue AndAlso MyBase.SelectedDate.Value <> value.Value) Then MyBase.SelectedDate = value
            If Me.SelectedDate.HasValue <> value.HasValue OrElse (Me.SelectedDate.HasValue AndAlso Me.SelectedDate.Value <> value.Value) Then SetValue(SelectedDateProperty, value)
            If Not _textBox Is Nothing AndAlso Me.IsKeyboardFocusWithin Then
                _textBox.SelectAll()
            End If
        End Set
    End Property

    Public Shadows Property IsTabStop() As Boolean
        Get
            Return GetValue(IsTabStopProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsTabStopProperty, value)
        End Set
    End Property

    Public Property HighlightedDates As List(Of DateTime)
        Get
            Return GetValue(HighlightedDatesProperty)
        End Get
        Set(value As List(Of DateTime))
            SetValue(HighlightedDatesProperty, value)
        End Set
    End Property

    Private Shared _minWidth As Double = Double.NaN
    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()

        _textBox = Me.Template.FindName("PART_TextBox", Me)
        _calendar = Me.Template.FindName("PART_Popup", Me).Child

        If Double.IsNaN(_minWidth) AndAlso Me.SelectedDateFormat = DatePickerFormat.Short Then
            Dim text As String = _textBox.Text
            _textBox.Text = String.Format("{0:d}", New DateTime(2018, 12, 31))
            _textBox.Measure(New Size(Double.PositiveInfinity, Double.PositiveInfinity))
            _minWidth = _textBox.DesiredSize.Width
            _textBox.Text = text
        End If
        _textBox.MinWidth = _minWidth

        AddHandler _textBox.TextChanged, AddressOf textBox_TextChanged
    End Sub

    Private _skip As Boolean = False
    Private _text As String = String.Empty
    Private Sub textBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not _skip Then
            Dim trace As String = New StackTrace().ToString()
            If (trace.Contains("Focus(") OrElse trace.Contains(".TogglePopUp()")) AndAlso (Not Me.SelectedDate.HasValue OrElse Me.SelectedDate.Value.Ticks = 0) Then
                ' clear textbox immediately, but only with empty text to avoid stack overflowing
                _skip = True
                _textBox.Text = String.Empty
                _skip = False

                ' the text is being set to an unwanted value after lost focus -- set it back (from outside this event handler)
                Application.Current.Dispatcher.BeginInvoke(
                    Sub()
                        _skip = True
                        _textBox.Text = _text
                        _calendar.DisplayDate = DateTime.Now  ' set the calendar to today in case it shows up while the selected date is set to invalid (1/1/1)
                        _skip = False
                    End Sub)
            Else
                Dim index As Integer = _textBox.SelectionStart
                Dim text As String = _textBox.Text

                ' if this TextChanged event is caused by the user actually typing or selecting a date on the calendar or pasting or cutting...
                If trace.Contains(".TextEditorTyping.") OrElse trace.Contains(".Calendar.OnSelectedDateChanged(") _
                    OrElse trace.Contains(".TextEditorCopyPaste.OnPaste(") OrElse trace.Contains(".TextEditorCopyPaste.OnCut(") Then
                    ' try parse the entered value
                    ' see https://stackoverflow.com/questions/3310569/what-is-the-significance-of-1-1-1753-in-sql-server
                    Dim dt As DateTime
                    If DateTime.TryParse(_textBox.Text, Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.None, dt) AndAlso dt.Year >= 1753 Then
                        Me.SelectedDate = dt
                        Me.IsInvalid = False
                    ElseIf _textBox.Text.Length = 4 AndAlso DateTime.TryParse(_textBox.Text.Substring(0, 2) + DateTimeFormatInfo.CurrentInfo.DateSeparator + _textBox.Text.Substring(2, 2), Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.None, dt) AndAlso dt.Year >= 1753 Then
                        Me.SelectedDate = dt
                        Me.IsInvalid = False
                    ElseIf _textBox.Text.Length = 6 AndAlso DateTime.TryParse(_textBox.Text.Substring(0, 2) + DateTimeFormatInfo.CurrentInfo.DateSeparator + _textBox.Text.Substring(2, 2) + DateTimeFormatInfo.CurrentInfo.DateSeparator + _textBox.Text.Substring(4, 2), Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.None, dt) AndAlso dt.Year >= 1753 Then
                        Me.SelectedDate = dt
                        Me.IsInvalid = False
                    ElseIf _textBox.Text.Length = 8 AndAlso DateTime.TryParse(_textBox.Text.Substring(0, 2) + DateTimeFormatInfo.CurrentInfo.DateSeparator + _textBox.Text.Substring(2, 2) + DateTimeFormatInfo.CurrentInfo.DateSeparator + _textBox.Text.Substring(4, 4), Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.None, dt) AndAlso dt.Year >= 1753 Then
                        Me.SelectedDate = dt
                        Me.IsInvalid = False
                    ElseIf String.IsNullOrWhiteSpace(text) Then
                        Me.SelectedDate = Nothing
                        Me.IsInvalid = False
                    Else
                        Me.SelectedDate = New DateTime(0)
                        Me.IsInvalid = True
                    End If

                    ' remember what the user typed
                    _text = text
                End If

                _textBox.Text = text
                _textBox.SelectionStart = index
            End If
        End If
    End Sub

    Public Property IsInvalid As Boolean
        Get
            Return GetValue(IsInvalidProperty)
        End Get
        Friend Set(value As Boolean)
            SetValue(IsInvalidPropertyKey, value)
        End Set
    End Property
End Class
