Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls

Friend Class WeekNumberTextBlock
    Inherits TextBlock

    Public Shared ReadOnly CalendarProperty As DependencyProperty =
        DependencyProperty.Register("Calendar", GetType(System.Windows.Controls.Calendar), GetType(WeekNumberTextBlock),
            New FrameworkPropertyMetadata(Nothing, AddressOf CalendarChanged))

    Public Shared ReadOnly NumberProperty As DependencyProperty =
        DependencyProperty.Register("Number", GetType(Integer?), GetType(WeekNumberTextBlock),
            New FrameworkPropertyMetadata(Nothing, AddressOf NumberChanged))

    Public Property Calendar As System.Windows.Controls.Calendar
        Get
            Return GetValue(CalendarProperty)
        End Get
        Set(ByVal value As System.Windows.Controls.Calendar)
            SetValue(CalendarProperty, value)
        End Set
    End Property

    Public Property Number As Integer?
        Get
            Return GetValue(NumberProperty)
        End Get
        Set(ByVal value As Integer?)
            SetValue(NumberProperty, value)
        End Set
    End Property

    Private Shared Sub CalendarChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim weekNumberTextBlock As WeekNumberTextBlock = d
        Dim oldCalendar As System.Windows.Controls.Calendar = e.OldValue
        Dim newCalendar As System.Windows.Controls.Calendar = e.NewValue

        If Not oldCalendar Is Nothing Then
            RemoveHandler oldCalendar.DisplayDateChanged, AddressOf weekNumberTextBlock.CalendarDateChanged
        End If
        AddHandler newCalendar.DisplayDateChanged, AddressOf weekNumberTextBlock.CalendarDateChanged

        weekNumberTextBlock.Recalc()
    End Sub

    Private Shared Sub NumberChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim weekNumberTextBlock As WeekNumberTextBlock = d
        weekNumberTextBlock.Recalc()
    End Sub

    Friend Sub CalendarDateChanged(sender As Object, e As CalendarDateChangedEventArgs)
        Recalc()
    End Sub

    Friend Sub Recalc()
        Try
            If Me.Number.HasValue AndAlso Not Me.Calendar Is Nothing Then
                Dim offset As Integer = 0
                Dim firstDayOfMonth As DateTime = New DateTime(Me.Calendar.DisplayDate.Year, Me.Calendar.DisplayDate.Month, 1)
                If firstDayOfMonth.DayOfWeek = Me.Calendar.FirstDayOfWeek Then
                    offset = -1
                End If

                Me.Text = getISO8601WeekOfYear(firstDayOfMonth.AddDays(7 * (Me.Number + offset)))
            End If
        Catch ex As Exception
            Me.Text = "??"
        End Try
    End Sub

    ' This presumes that weeks start with Monday.
    ' Week 1 Is the 1st week of the year with a Thursday in it.
    Private Function getISO8601WeekOfYear(dt As DateTime) As Integer
        ' Seriously cheat.  If its Monday, Tuesday Or Wednesday, then it'll 
        ' be the same week# as whatever Thursday, Friday Or Saturday are,
        ' And we always get those right
        Dim day As DayOfWeek = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt)
        If (day >= DayOfWeek.Monday AndAlso day <= DayOfWeek.Wednesday) Then
            dt = dt.AddDays(3)
        End If

        ' Return the week of our adjusted day
        Return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
    End Function
End Class
