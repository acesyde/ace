﻿using ACE.Demo.Repositories;
using ACE;
using Grit.Sequence;
using Grit.Sequence.Repository.MySql;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Loggers;
using ACE.WS;

namespace ACE.Demo.Heavy.Web
{
    public static class BootStrapper
    {
        public static Ninject.IKernel Container { get; private set; }
        public static EasyNetQ.IBus EasyNetQBus { get; private set; }
        public static void BootStrap()
        {
            Container = new StandardKernel();

            RabbitHutch.SetContainerFactory(() => { return new EasyNetQ.DI.NinjectAdapter(Container); });
            EasyNetQBus = EasyNetQ.RabbitHutch.CreateBus(Grit.Configuration.RabbitMQ.ACEQueueConnectionString,
                x => x.Register<IEasyNetQLogger, NullLogger>());

            BindFrameworkObjects();
            BindBusinessObjects();
        }

        private static void BindFrameworkObjects()
        {
            Container.Settings.AllowNullInjection = true;

            Container.Bind<ACE.Loggers.IBusLogger>().To<ACE.Loggers.Log4NetBusLogger>().InSingletonScope();

            // EventBus must be thread scope, published events will be saved in thread EventBus._events, until Flush/Clear.
            Container.Bind<IEventBus>().To<EventBus>()
                .InThreadScope();

            // ActionBus must be thread scope, single thread bind to use single anonymous RabbitMQ queue for reply.
            Container.Bind<IActionBus>().To<ActionBus>().InThreadScope();

            IServiceMappingFactory serviceMappingFactory = new ServiceMappingFactory(() => {
                return new Dictionary<Type, ServiceMapping>() {
                     { typeof(ACE.Demo.Contracts.Services.GetInvestmentsRequest), new ServiceMapping("http://localhost:59857", "api/investment/list") },
                     { typeof(ACE.Demo.Contracts.Services.GetInvestmentRequest), new ServiceMapping("http://localhost:59857", "api/investment") }
                };
            });
            Container.Bind<IServiceMappingFactory>().ToConstant(serviceMappingFactory);
            Container.Bind<IServiceBus>().To<ServiceBus>().InSingletonScope();
        }

        private static void BindBusinessObjects()
        {
            Container.Bind<ISequenceRepository>().To<SequenceRepository>().InSingletonScope();
            Container.Bind<ISequenceService>().To<SequenceService>().InSingletonScope();
        }

        public static void Dispose()
        {
            if (EasyNetQBus != null)
            {
                EasyNetQBus.Dispose();
            }
            if (Container != null)
            {
                Container.Dispose();
            }
        }
    }
}
