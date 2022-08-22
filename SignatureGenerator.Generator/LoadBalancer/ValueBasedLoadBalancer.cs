using SignatureGenerator.Generator.LoadBalancer.Abstractions;
using System.Collections.Generic;
using System.Threading;

namespace SignatureGenerator.Generator.LoadBalancer
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class ValueBasedLoadBalancer : IValuesLoadBalancer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="events">Events to be stoped by condition</param>
        /// <param name="controllSum">Sum of elements in produced data</param>
        /// <param name="sumForCompare">Sum of elements in consumed data</param>
        public void BalanceByManualEvents(IReadOnlyCollection<ManualResetEvent> events, int controllSum, int sumForCompare)
        {   
            if(!IsEnough(controllSum, sumForCompare))
            {
                foreach (var @event in events)
                    @event.Reset();
            }
            else
            {
                foreach (var @event in events)
                    @event.Set();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="events">Events to be stoped by condition</param>
        /// <param name="controllSum">Sum of elements in produced data</param>
        /// <param name="sumForCompare">Sum of elements in consumed data</param>
        public void BalanceByManualEvents(IReadOnlyCollection<ManualResetEventSlim> events, int controllSum, int sumForCompare)
        {
            if (!IsEnough(controllSum, sumForCompare))
            {
                foreach (var @event in events)
                    @event.Reset();
            }
            else
            {
                foreach (var @event in events)
                    @event.Set();
            }
        }


        private bool IsEnough(int controllSum, int sumForCompare)
        {
            return controllSum == 0
                || controllSum <= sumForCompare
                && 100 / ((double)sumForCompare / (double)(sumForCompare - controllSum)) <= 10;
        }
    }
}
