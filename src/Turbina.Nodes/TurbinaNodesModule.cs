using System;
using Autofac;

namespace Turbina.Nodes
{
    public class TurbinaNodesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => typeof(ConsoleOutputNode)).As<Type>().Named<Type>("NodeTypes");
            builder.Register(context => typeof(HttpRequestNode)).As<Type>().Named<Type>("NodeTypes");
            builder.Register(context => typeof(RandomNode)).As<Type>().Named<Type>("NodeTypes");
            builder.Register(context => typeof(StringConcatNode)).As<Type>().Named<Type>("NodeTypes");
            builder.Register(context => typeof(StringConstNode)).As<Type>().Named<Type>("NodeTypes");
            builder.Register(context => typeof(TimerNode)).As<Type>().Named<Type>("NodeTypes");
        }
    }
}