Imports System.Windows.Data

Friend Class DateIsInListConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        If values.Length < 2 OrElse Not TypeOf values(0) Is DateTime OrElse Not TypeOf values(1) Is IEnumerable(Of DateTime) Then
            Return False
        End If

        Dim dt As DateTime = values(0)
        Dim dtList As IEnumerable(Of DateTime) = values(1)

        Return dtList.Contains(dt)
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class
