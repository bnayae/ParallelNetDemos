using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncWhenAll
{
    public static class Extensions
    {
        /// <summary>
        /// Throws all (when catching exception withing async / await
        /// and there is potential for multiple exception to be thrown.
        /// async / await will propagate single exception.
        /// in order to catch all the exception use this extension).
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Task ThrowAll(this Task t)
        {
            var result = t.ContinueWith(c =>
            {
                if (t.IsCanceled)
                    throw new OperationCanceledException();
                if (c.Exception == null)
                    return;
                throw new AggregateException(c.Exception.Flatten());
            });
            return result;
        }

        /// <summary>
        /// Throws all (when catching exception withing async / await
        /// and there is potential for multiple exception to be thrown.
        /// async / await will propagate single exception.
        /// in order to catch all the exception use this extension).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Task<T> ThrowAll<T>(this Task<T> t)
        {
            var result = t.ContinueWith(c =>
            {
                if (t.IsCanceled)
                    throw new OperationCanceledException();
                if (c.Exception == null)
                    return c.Result;
                throw new AggregateException(c.Exception.Flatten());
            });
            return result;
        }
    }
}