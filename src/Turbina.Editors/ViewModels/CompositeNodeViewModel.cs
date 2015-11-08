using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Turbina.Editors.ViewModels
{
    public class CompositeNodeViewModel : NodeViewModel
    {
        private readonly ObservableCollection<LinkViewModel> _linkViewModels = new ObservableCollection<LinkViewModel>();
        private readonly ObservableCollection<NodeViewModel> _innerNodeViewModels = new ObservableCollection<NodeViewModel>();
        private readonly ObservableCollection<NodeViewModel> _selectedNodeViewModels = new ObservableCollection<NodeViewModel>();
        private double _scale;
        private Point _topLeftCorner;
        private LinkViewModel _currentUnfinishedLink;

        public CompositeNodeViewModel(CompositeNode node, Vector location, IControlTypesResolver controlTypesResolver)
            : base(node, location, controlTypesResolver)
        {
            _scale = 1.0;
        }

        public new CompositeNode Node => (CompositeNode) base.Node;

        public double Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged();
                }
            }
        }

        public Point TopLeftCorner
        {
            get { return _topLeftCorner; }
            set
            {
                if (_topLeftCorner != value)
                {
                    _topLeftCorner = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<LinkViewModel> LinkViewModels => _linkViewModels;

        public IReadOnlyList<NodeViewModel> InnerNodeViewModels => _innerNodeViewModels;

        public IReadOnlyList<NodeViewModel> SelectedNodeViewModels => _selectedNodeViewModels;

        public void AddNode(Node node, Vector location)
        {
            var nodeViewModel = new NodeViewModel(node, location, ControlTypesResolver);
            nodeViewModel.PropertyChanged += OnNodeViewModelPropertyChanged;
            _innerNodeViewModels.Add(nodeViewModel);
            Node.AddNode(node);
        }

        public void RemoveNode(Node node)
        {
            var nodeViewModel = _innerNodeViewModels.FirstOrDefault(model => model.Node == node);
            if (nodeViewModel != null)
            {
                nodeViewModel.PropertyChanged -= OnNodeViewModelPropertyChanged;
                nodeViewModel.Dispose();
                _innerNodeViewModels.Remove(nodeViewModel);
                Node.RemoveNode(node);
            }
        }

        public void Link(Node sourceNode, IPin sourcePin, Node targetNode, IPin targetPin)
        {
            var linkViewModel = new LinkViewModel(
                GetNodeViewModel(sourceNode).GetPinViewModel(sourcePin),
                GetNodeViewModel(targetNode).GetPinViewModel(targetPin),
                ControlTypesResolver);
            _linkViewModels.Add(linkViewModel);
        }

        public void BringToFront(Node node)
        {
            var idx = _innerNodeViewModels.IndexOf(GetNodeViewModel(node));
            if (idx >= 0)
            {
                _innerNodeViewModels.Move(idx, _innerNodeViewModels.Count - 1);
            }
        }

        public NodeViewModel GetNodeViewModel(Node node)
        {
            return _innerNodeViewModels.First(model => model.Node == node);
        }

        public void ResetNodes()
        {
            Node.ResetNodes();
        }

        private void OnNodeViewModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            var viewModel = (NodeViewModel)sender;

            switch (eventArgs.PropertyName)
            {
                case nameof(NodeViewModel.IsSelected):
                    if (viewModel.IsSelected)
                    {
                        _selectedNodeViewModels.Add(viewModel);
                    }
                    else
                    {
                        _selectedNodeViewModels.Remove(viewModel);
                    }

                    break;
            }
        }

        public CanvasPoint BeginLinkage(PinViewModel pinViewModel)
        {
            _currentUnfinishedLink = new LinkViewModel(pinViewModel, null, ControlTypesResolver);
            _linkViewModels.Add(_currentUnfinishedLink);

            return _currentUnfinishedLink.EndPoint;
        }

        public CanvasPoint BeginRelinkage(PinViewModel pinViewModel)
        {
            _currentUnfinishedLink = _linkViewModels.First(model => model.TargetPinViewModel == pinViewModel);
            _currentUnfinishedLink.TargetPinViewModel = null;
            var canvasPoint = new CanvasPoint();
            _currentUnfinishedLink.EndPoint = canvasPoint;
            return canvasPoint;
        }

        public void EndLinkage(PinViewModel pinViewModel)
        {
            if (_currentUnfinishedLink != null)
            {
                var success = false;
                if (_currentUnfinishedLink.TargetPinViewModel == null)
                {
                    // Can't link to nowhere
                    if (pinViewModel != null)
                    {
                        // Can't link input<->input
                        if (pinViewModel.Pin.Direction == PinDirection.Input)
                        {
                            // Can't link node to itself
                            if (pinViewModel.NodeViewModel != _currentUnfinishedLink.SourcePinViewModel.NodeViewModel)
                            {
                                // Hurray! Successfully linked!

                                // Cleanup previous loosers
                                foreach (var linkViewModel in _linkViewModels.Where(model => model.TargetPinViewModel == pinViewModel).ToArray())
                                {
                                    Node.Unlink(linkViewModel.TargetPinViewModel.NodeViewModel.Node, linkViewModel.TargetPinViewModel.Pin);
                                    _linkViewModels.Remove(linkViewModel);
                                }

                                _currentUnfinishedLink.TargetPinViewModel = pinViewModel;
                                Node.Link(
                                    _currentUnfinishedLink.SourcePinViewModel.NodeViewModel.Node,
                                    _currentUnfinishedLink.SourcePinViewModel.Pin,
                                    pinViewModel.NodeViewModel.Node,
                                    pinViewModel.Pin);

                                pinViewModel.NodeViewModel.Node.Pulse();
                                success = true;
                            }
                        }
                    }
                }

                if (!success)
                {
                    _linkViewModels.Remove(_currentUnfinishedLink);
                }

                _currentUnfinishedLink = null;
            }
        }
    }
}