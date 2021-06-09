using System.Threading.Tasks;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ
{
    public interface IActiveConsumerListener
    {
        Task ConsumeAsync(RecieveResult recieveResult);
    }
}
