using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PizzaOrder.Business.Models;

namespace PizzaOrder.Business.Services
{
    public interface IEventService
    {
        IObservable<EventDataModel> OnCreateObservable();

        void CreateOrderEvent(EventDataModel orderEvent);
        void StatusUpdateEvent(EventDataModel orderEvent);
        IObservable<EventDataModel> OnStatusUpdateOnservable();
    }

    public class EventService : IEventService
    {
        #region Create Event

        private readonly ISubject<EventDataModel> _onCreateSubject = new ReplaySubject<EventDataModel>(1);

        public void CreateOrderEvent(EventDataModel orderEvent) => _onCreateSubject.OnNext(orderEvent);

        public IObservable<EventDataModel> OnCreateObservable() => _onCreateSubject.AsObservable();

        #endregion

        #region StatusUpdate Event

        private readonly ISubject<EventDataModel> _onStatusUpdateSubject = new ReplaySubject<EventDataModel>(1);

        public void StatusUpdateEvent(EventDataModel orderEvent) => _onStatusUpdateSubject.OnNext(orderEvent);

        public IObservable<EventDataModel> OnStatusUpdateOnservable() => _onStatusUpdateSubject.AsObservable();

        #endregion
    }
}
