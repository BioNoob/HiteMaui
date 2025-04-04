using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Maui.Graphics;
namespace HiteMaui.Models
{
    public class CommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public CommandHandler(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
    public class ColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            //color to str
            if(value != null)
            {
                return Color.Parse(value.ToString());
                return ((Color)value).ToRgbString();
            }
            else return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            //str to color
            if(value != null )
            {
                return ((Color)value).ToRgbString();
                return Color.Parse(value.ToString());
            }
            else return null;
        }
    }
}
