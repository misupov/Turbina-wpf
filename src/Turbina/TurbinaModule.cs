using System;
using Autofac;

namespace Turbina
{
    public class TurbinaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => typeof(CompositeNode)).As<Type>().Named<Type>("NodeTypes");
        }
    }
}