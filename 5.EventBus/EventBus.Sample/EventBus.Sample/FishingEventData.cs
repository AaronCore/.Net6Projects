using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus;

namespace EventBusApp.Sample
{
    public class FishingEventData : EventData
    {
        public FishType FishType { get; set; }
        public FishingMan FishingMan { get; set; }
    }
}
