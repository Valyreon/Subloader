using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SubloaderWpf.Mvvm;

public abstract class ObservableEntity : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
        RaisePropertyChanged(GetName(propertyExpression));
    }

    protected void Set<T>(Expression<Func<T>> propertyExpression, ref T field, T value)
    {
        field = value;
        RaisePropertyChanged(GetName(propertyExpression));
    }

    private static string GetName<T>(Expression<Func<T>> exp)
    {
        if (exp.Body is not MemberExpression body)
        {
            var ubody = (UnaryExpression)exp.Body;
            body = ubody.Operand as MemberExpression;
        }

        return body.Member.Name;
    }

    private void RaisePropertyChanged(string property)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
