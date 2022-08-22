using System.Collections.Generic;
using System.Threading;

namespace SignatureGenerator.Generator.LoadBalancer.Abstractions
{
    /// <summary>
    /// Load balancer based on 2 values
    /// </summary>
    public interface IValuesLoadBalancer
    {
        /// <summary>
        /// Balances load throught manual reset event
        /// </summary>
        /// <param name="events">Events to be stoped by condition</param>
        /// <param name="controllSum">Sum of elements in produced data</param>
        /// <param name="sumForCompare">Sum of elements in consumed data</param>
        public void BalanceByManualEvents(IReadOnlyCollection<ManualResetEvent> events, int controllSum, int sumForCompare);

        /// <summary>
        /// Balances load throught manual reset event
        /// </summary>
        /// <param name="events">Events to be stoped by condition</param>
        /// <param name="controllSum">Sum of elements in produced data</param>
        /// <param name="sumForCompare">Sum of elements in consumed data</param>
        public void BalanceByManualEvents(IReadOnlyCollection<ManualResetEventSlim> events, int controllSum, int sumForCompare);
    }
}
