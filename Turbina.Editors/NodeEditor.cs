using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Turbina.Editors.PinControls;

namespace Turbina.Editors
{
    [TemplatePart(Name = "PART_NodePropertiesPanel", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Thumb", Type = typeof(ThumbWithContent))]
    [TemplatePart(Name = "PART_InputPins", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_OutputPins", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_ConnectionPointsPanel", Type = typeof(PinPanel))]
    [TemplatePart(Name = "PART_PlayPause", Type = typeof(PlayPauseButton))]
    [EditorFor(typeof(Node))]
    public class NodeEditor : Control, ISelectable, ICollapsible, IPinConnectable
    {
        private FrameworkElement _nodePropertiesPanel;
//        private ThumbWithContent _thumb;
//        private ItemsControl _inputPins;
//        private ItemsControl _outputPins;
//        private PinPanel _pinPanel;
        private PlayPauseButton _playPauseButton;

        static NodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeEditor), new FrameworkPropertyMetadata(typeof(NodeEditor)));
        }

        #region [DP] public string Title { get; set; }

        public static readonly DependencyProperty TitleProperty = DependencyProperty<NodeEditor>.Register(
            control => control.Title,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region [DP] public ObservableCollection<Pin> InputPins { get; }

        private static readonly DependencyPropertyKey InputPinsPropertyKey = DependencyProperty<NodeEditor>.RegisterReadOnly(control => control.InputPins);
        public static readonly DependencyProperty InputPinsProperty = InputPinsPropertyKey.DependencyProperty;

        public ObservableCollection<Pin> InputPins
        {
            get { return (ObservableCollection<Pin>)GetValue(InputPinsProperty); }
            private set { SetValue(InputPinsPropertyKey, value); }
        }

        #endregion

        #region [DP] public ObservableCollection<Pin> OutputPins { get; }

        private static readonly DependencyPropertyKey OutputPinsPropertyKey = DependencyProperty<NodeEditor>.RegisterReadOnly(control => control.OutputPins);
        public static readonly DependencyProperty OutputPinsProperty = OutputPinsPropertyKey.DependencyProperty;

        public ObservableCollection<Pin> OutputPins
        {
            get { return (ObservableCollection<Pin>)GetValue(OutputPinsProperty); }
            private set { SetValue(OutputPinsPropertyKey, value); }
        }

        #endregion

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

        public NodeEditor()
        {
            InputPins = new ObservableCollection<Pin>();
            OutputPins = new ObservableCollection<Pin>();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            InputPins.Clear();
            OutputPins.Clear();
            Title = null;

            var oldNode = eventArgs.OldValue as Node;

            if (oldNode != null)
            {
                oldNode.Processed -= OnNodeProcessed;
            }

            Func<IPin, PinEditorControl> inputControlFactory = InputControlFactory;
            Func<IPin, PinEditorControl> outputControlFactory = OutputControlFactory;
            if (Node != null)
            {
                foreach (var pin in Node.InputPins)
                {
                    InputPins.Add(new Pin(inputControlFactory(pin.Value), pin.Value, Node, FlowDirection.LeftToRight) {Title = pin.Key});
                }

                foreach (var pin in Node.OutputPins)
                {
                    OutputPins.Add(new Pin(outputControlFactory(pin.Value), pin.Value, Node, FlowDirection.RightToLeft) {Title = pin.Key});
                }

                Node.Processed += OnNodeProcessed;
                Title = Node.Title;
            }
        }

        private PinEditorControl InputControlFactory(IPin pin)
        {
            if (pin.Type == typeof (string))
            {
                return new TextBoxPinControl();
            }
            else if (pin.Type == typeof(bool))
            {
                return new OnOffPinControl();
            }
            else if (pin.Type == typeof(TimeSpan))
            {
                return new TimeSpanPinControl();
            }
            else
            {
                return new TextBlockPinControl();
            }
        }

        private PinEditorControl OutputControlFactory(IPin pin)
        {
            return new TextBlockPinControl();
        }

        HashSet<Pin> pinsInUpdatingState = new HashSet<Pin>();

        private void OnNodeProcessed(object sender, EventArgs eventArgs)
        {
            Dispatcher.InvokeAsync(() =>
            {
                foreach (var pin in InputPins)
                {
                    if (pinsInUpdatingState.Add(pin))
                    {
                        pin.Update();
                        pinsInUpdatingState.Remove(pin);
                    }
                }

                foreach (var pin in OutputPins)
                {
                    if (pinsInUpdatingState.Add(pin))
                    {
                        pin.Update();
                        pinsInUpdatingState.Remove(pin);
                    }
                }
            });
        }

        public Node Node => (Node) DataContext;

        public Point GetInputPinCoordinates(Pin pin)
        {
            var result = new Point(0, 0);
            var offsetPoint = pin.TranslatePoint(new Point(), this);
            result.Offset(offsetPoint.X, offsetPoint.Y + pin.ActualHeight/2);

            return result;
        }

        public Point GetOutputPinCoordinates(Pin pin)
        {
            var result = new Point(0, 0);
            var offsetPoint = pin.TranslatePoint(new Point(), this);
            result.Offset(offsetPoint.X, offsetPoint.Y + pin.ActualHeight / 2);

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
            Node.Pulse();
        }
    }
}