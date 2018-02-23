
namespace LSports.Scheduler.Jobs
{
    using Quartz;
    using System.Linq;
    using LSports.Scheduler.Jobs.@base;

	/// <summary>
	/// Launching worker for selected products(if param === false ? pull-products : push-products)
	/// </summary>
	[DisallowConcurrentExecution]
    public class XmlPullCommonWorker : XmlCommonWorker
    {
        public override void Run()
        {
			// only pull products
            checkingProductIds = productRepository.GetListByType(false).Select(x=>x.Id).ToList();

            ProcessArrivalMessages();
        }
    }
}