//using System;
//using System.Linq;
//using Turbina.Nodes;
//
//namespace Turbina.UI
//{
//    public class TestNodes
//    {
//        public static Node Test1()
//        {
//            var resultNode = new CompositeNode {Title = nameof(Test1)};
//
//            var httpRequestNode = new HttpRequestNode { Uri = new Uri("http://ya.ru") };
//            var consoleOutputNode = new ConsoleOutputNode();
//
//            resultNode.AddNode(httpRequestNode);
//            resultNode.AddNode(consoleOutputNode);
//
//            resultNode.Link(
//                httpRequestNode,
//                httpRequestNode.OutputPins[nameof(httpRequestNode.Result)],
//                consoleOutputNode,
//                consoleOutputNode.InputPins[nameof(consoleOutputNode.Input)]);
//
//            return resultNode;
//        }
//
//        private static Node Test2()
//        {
//            var resultNode = new CompositeNode { Title = nameof(Test2) };
//
//            //
//            //   [b]------------------------                            [b]-----------------------------
//            //         \                    \                                 \                         \
//            //         [1.+]---              \                                [ba]--                     \
//            //         /       \              \                               /     \                     \
//            //   [a]---      [3.+]---         [6.+]---[out]     =>      [a]---      [baac]---             [bbaacacc]---[out]
//            //         \     /       \        /                               \     /        \            /
//            //         [2.+]---      [5.+]----                                [ac]--         [baacacc]----
//            //         /     \       /                                        /     \        /
//            //   [c]---------[4.+]---                                   [c]---------[acc]----
//            //
//            //
//            //
//            //
//            //
//
//            var a = new StringConstNode {Value = "a", Title = "a"};
//            var b = new StringConstNode {Value = "b", Title = "b"};
//            var c = new StringConstNode {Value = "c", Title = "c"};
//
//            var n1 = new StringConcatNode {Title = "n1"};
//            var n2 = new StringConcatNode {Title = "n2"};
//            var n3 = new StringConcatNode {Title = "n3"};
//            var n4 = new StringConcatNode {Title = "n4"};
//            var n5 = new StringConcatNode {Title = "n5"};
//            var n6 = new StringConcatNode {Title = "n6"};
//
//            var consoleOutputNode = new ConsoleOutputNode {Title = "[out]"};
//
//            resultNode.AddNode(a);
//            resultNode.AddNode(b);
//            resultNode.AddNode(c);
//            resultNode.AddNode(n1);
//            resultNode.AddNode(n2);
//            resultNode.AddNode(n3);
//            resultNode.AddNode(n4);
//            resultNode.AddNode(n5);
//            resultNode.AddNode(n6);
//            resultNode.AddNode(consoleOutputNode);
//
//            resultNode.Link(a, a.OutputPins[nameof(a.Value)], n1, n1.InputPins[nameof(n1.Right)]);
//            resultNode.Link(b, b.OutputPins[nameof(b.Value)], n1, n1.InputPins[nameof(n1.Left)]);
//            resultNode.Link(a, a.OutputPins[nameof(a.Value)], n2, n2.InputPins[nameof(n2.Left)]);
//            resultNode.Link(c, c.OutputPins[nameof(c.Value)], n2, n2.InputPins[nameof(n2.Right)]);
//            resultNode.Link(n1, n1.OutputPins[nameof(n1.Value)], n3, n3.InputPins[nameof(n3.Left)]);
//            resultNode.Link(n2, n2.OutputPins[nameof(n2.Value)], n3, n3.InputPins[nameof(n3.Right)]);
//            resultNode.Link(n2, n2.OutputPins[nameof(n2.Value)], n4, n4.InputPins[nameof(n4.Left)]);
//            resultNode.Link(c, c.OutputPins[nameof(c.Value)], n4, n4.InputPins[nameof(n4.Right)]);
//            resultNode.Link(b, b.OutputPins[nameof(b.Value)], n6, n6.InputPins[nameof(n6.Left)]);
//            resultNode.Link(n3, n3.OutputPins[nameof(n3.Value)], n5, n5.InputPins[nameof(n5.Left)]);
//            resultNode.Link(n4, n4.OutputPins[nameof(n4.Value)], n5, n5.InputPins[nameof(n5.Right)]);
//            resultNode.Link(n5, n5.OutputPins[nameof(n5.Value)], n6, n6.InputPins[nameof(n6.Right)]);
//            resultNode.Link(n6, n6.OutputPins[nameof(n6.Value)], consoleOutputNode, consoleOutputNode.InputPins[nameof(consoleOutputNode.Input)]);
//
//            return resultNode;
//        }
//
//        public static Node Test3(NodeDispatcher dispatcher = null)
//        {
//            var resultNode = new CompositeNode(dispatcher) { Title = nameof(Test3) };
//
//            var node1 = Test1();
//            var node2 = Test2();
//            var node3 = new StringConcatNode();
//            var consoleOutputNode = new ConsoleOutputNode();
//
//            resultNode.AddNode(node1);
//            resultNode.AddNode(node2);
//            resultNode.AddNode(node3);
//            resultNode.AddNode(consoleOutputNode);
//
//            resultNode.Link(node1, node1.OutputPins.First().Value, node3, node3.InputPins[nameof(node3.Left)]);
//            resultNode.Link(node2, node2.OutputPins.First().Value, node3, node3.InputPins[nameof(node3.Right)]);
//            resultNode.Link(node3, node3.OutputPins[nameof(node3.Value)], consoleOutputNode, consoleOutputNode.InputPins[nameof(consoleOutputNode.Input)]);
//
//            return resultNode;
//        }
//
//        public static Node Test4(NodeDispatcher dispatcher)
//        {
//            var resultNode = new CompositeNode(dispatcher);
//
//            var timerNode = new TimerNode {Dispatcher = new NodeDispatcher("Timer Dispatcher")};
//            var randomNode = new RandomNode();
//            var consoleOutputNode = new ConsoleOutputNode { Dispatcher = new NodeDispatcher("Console Dispatcher") };
//
//            timerNode.Interval = TimeSpan.FromSeconds(1);
//            timerNode.IsEnabled = true;
//
//            resultNode.AddNode(timerNode);
//            resultNode.AddNode(randomNode);
//            resultNode.AddNode(consoleOutputNode);
//
//            resultNode.Link(
//                timerNode,
//                timerNode.OutputPins[nameof(timerNode.Counter)],
//                randomNode,
//                randomNode.InputPins[nameof(randomNode.Trigger)]);
//
//
//            resultNode.Link(
//                randomNode,
//                randomNode.OutputPins[nameof(randomNode.Value)],
//                consoleOutputNode,
//                consoleOutputNode.InputPins[nameof(consoleOutputNode.Input)]);
//
//            return resultNode;
//        }
//    }
//}