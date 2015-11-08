﻿using System.Windows;
using System.Windows.Input;

namespace Turbina.Editors.Behaviors
{
    public class MouseDown
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(MouseDown),
                new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                typeof(object),
                typeof(MouseDown),
                new UIPropertyMetadata(null));

            public static void SetCommand(DependencyObject target, ICommand value)
            {
                target.SetValue(CommandProperty, value);
            }

            public static void SetCommandParameter(DependencyObject target, object value)
            {
                target.SetValue(CommandParameterProperty, value);
            }

            public static object GetCommandParameter(DependencyObject target)
            {
                return target.GetValue(CommandParameterProperty);
            }

            private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
            {
                var element = target as FrameworkElement;
                if (element != null)
                {
                    if ((e.NewValue != null) && (e.OldValue == null))
                    {
                        element.MouseDown += OnMouseDown;
                    }
                    else if ((e.NewValue == null) && (e.OldValue != null))
                    {
                        element.MouseDown -= OnMouseDown;
                    }
                }
            }

            private static void OnMouseDown(object sender, RoutedEventArgs e)
            {
                var control = (FrameworkElement)sender;
                var command = (ICommand)control.GetValue(CommandProperty);
                var commandParameter = control.GetValue(CommandParameterProperty);
                command.Execute(commandParameter);
            }
        }
}