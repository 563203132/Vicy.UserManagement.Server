using System;
using Microsoft.Extensions.DependencyInjection;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.Api
{
    public class ASPNETEventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public ASPNETEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var handlerType = typeof(IHandler<>).MakeGenericType(domainEvent.GetType());
            foreach (dynamic handler in _serviceProvider.GetServices(handlerType))
            {
                handler.Handle((dynamic)domainEvent);
            }
        }
    }
}
