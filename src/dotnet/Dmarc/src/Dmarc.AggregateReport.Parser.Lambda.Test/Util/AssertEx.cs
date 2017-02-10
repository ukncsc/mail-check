using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Util
{
    public static class AssertEx
    {
        //Credited to Srikanth Venugopalan : http://stackoverflow.com/questions/15634542/nunit-async-test-exception-assertion
        public static async Task ThrowsAsync<TException>(Func<Task> func) where TException : class
        {
            await ThrowsAsync<TException>(func, exception => { });
        }

        public static async Task ThrowsAsync<TException>(Func<Task> func, Action<TException> action) where TException : class
        {
            var exception = default(TException);
            var expected = typeof(TException);
            Type actual = null;
            try
            {
                await func();
            }
            catch (Exception e)
            {
                exception = e as TException;
                actual = e.GetType();
            }

            Assert.AreEqual(expected, actual);
            action(exception);
        }
    }
}
