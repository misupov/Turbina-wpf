using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Turbina.Editors.ViewModels;

namespace Turbina.Editors
{
    [TemplatePart(Name = "PART_NodePropertiesPanel", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_PlayPause", Type = typeof(PlayPauseButton))]
    [EditorFor(typeof(Node))]
    public class NodeEditor : Control //, ISelectable, ICollapsible, IPinConnectable
    {
        private FrameworkElement _nodePropertiesPanel;
        private PlayPauseButton _playPauseButton;
        private readonly HashSet<Pin> _pinsInUpdatingState = new HashSet<Pin>();

        static NodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeEditor), new FrameworkPropertyMetadata(typeof(NodeEditor)));
        }

        #region [DP] public bool IsCollapsed { get; set; }
        
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty<NodeEditor>.Register(
            control => control.IsCollapsed,
            control => control.IsCollapsedChanged);

        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }
        
        private void IsCollapsedChanged(DependencyPropertyChangedEventArgs<bool> e)
        {
            var nodePropertiesPanel = _nodePropertiesPanel;

            if (nodePropertiesPanel == null)
            {
                return;
            }

            DoubleAnimation doubleAnimation;
            if (!e.NewValue)
            {
                nodePropertiesPanel.Visibility = Visibility.Visible;
                doubleAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(500)))
                {
                    EasingFunction = new ElasticEase {Oscillations = 2, Springiness = 10}
                };
            }
            else
            {
                doubleAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(300)))
                {
                    EasingFunction = new CubicEase {EasingMode = EasingMode.EaseOut}
                };
                doubleAnimation.Completed += (sender, args) =>
                {
                    if (nodePropertiesPanel.Opacity <= 0)
                    {
                        nodePropertiesPanel.Visibility = Visibility.Collapsed;
                    }
                };
            }
            
            var scaleTransform = (nodePropertiesPanel.LayoutTransform as ScaleTransform);
            if (scaleTransform == null)
            {
                scaleTransform = new ScaleTransform();
                nodePropertiesPanel.LayoutTransform = scaleTransform;
            }
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
            nodePropertiesPanel.BeginAnimation(OpacityProperty, doubleAnimation);
//            _pinPanel.BeginAnimation(PinPanel.CollapseCoefficientProperty, doubleAnimation);
        }

        #endregion

        public NodeViewModel ViewModel => (NodeViewModel) DataContext;

        public NodeEditor()
        {
//            InputPins = new ObservableCollection<Pin>();
//            OutputPins = new ObservableCollection<Pin>();

//            DataContextChanged += OnDataContextChanged;
        }

//        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
//        {
//            InputPins.Clear();
//            OutputPins.Clear();
//            Title = null;
//
//            var oldNode = eventArgs.OldValue as Node;
//
//            if (oldNode != null)
//            {
//                oldNode.Processed -= OnNodeProcessed;
//            }
//
//            Func<IPin, PinEditorControl> inputControlFactory = InputControlFactory;
//            Func<IPin, PinEditorControl> outputControlFactory = OutputControlFactory;
//            if (Node != null)
//            {
//                foreach (var pin in Node.InputPins)
//                {
//                    InputPins.Add(new Pin(inputControlFactory(pin.Value), pin.Value, Node, true));
//                }
//
//                foreach (var pin in Node.OutputPins)
//                {
//                    OutputPins.Add(new Pin(outputControlFactory(pin.Value), pin.Value, Node, false));
//                }
//
//                Node.Processed += OnNodeProcessed;
//                Title = Node.Title;
//                Node.Pulse();
//            }
//        }

//        private void OnNodeProcessed(object sender, EventArgs eventArgs)
//        {
//            Dispatcher.InvokeAsync(() =>
//            {
//                foreach (var pin in InputPins)
//                {
//                    if (_pinsInUpdatingState.Add(pin))
//                    {
//                        pin.Update();
//                        _pinsInUpdatingState.Remove(pin);
//                    }
//                }
//
//                foreach (var pin in OutputPins)
//                {
//                    if (_pinsInUpdatingState.Add(pin))
//                    {
//                        pin.Update();
//                        _pinsInUpdatingState.Remove(pin);
//                    }
//                }
//            }, DispatcherPriority.Background);
//        }

        public Point GetPinPoint(Pin pin, FrameworkElement relativeTo)
        {
            var result = pin.GetPinPoint(relativeTo);
            var translatePoint = TranslatePoint(new Point(pin.ViewModel.Pin.Direction == PinDirection.Input ? 4 : ActualWidth - 4, ActualHeight/2), relativeTo);
            result.Y = (translatePoint.Y * (1 - _nodePropertiesPanel.Opacity)) + result.Y * (_nodePropertiesPanel.Opacity);
            result.X = (translatePoint.X * (1 - _nodePropertiesPanel.Opacity)) + result.X * (_nodePropertiesPanel.Opacity);
            return result;
        }
        
        public override void OnApplyTemplate()
        {
            _nodePropertiesPanel = (FrameworkElement) GetTemplateChild("PART_NodePropertiesPanel");
//            _inputPins = (ItemsControl) GetTemplateChild("PART_InputPins");
//            _outputPins = (ItemsControl) GetTemplateChild("PART_OutputPins");
//            _pinPanel = (PinPanel) GetTemplateChild("PART_ConnectionPointsPanel");
            _playPauseButton = (PlayPauseButton)GetTemplateChild("PART_PlayPause");

            _nodePropertiesPanel.LayoutTransform = new ScaleTransform(IsCollapsed ? 0 : 1, IsCollapsed ? 0 : 1);
//            _pinPanel.CollapseCoefficient = IsCollapsed ? 0 : 1;

            _playPauseButton.Checked += PlayPauseButtonOnChecked;

            base.OnApplyTemplate();
        }

        private void PlayPauseButtonOnChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.Pulse();
        }
    }
}