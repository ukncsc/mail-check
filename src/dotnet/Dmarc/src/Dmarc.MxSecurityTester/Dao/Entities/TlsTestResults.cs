using System.Collections.Generic;
using System.Linq;

namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class TlsTestResults
    {
        public TlsTestResults(
            int failureCount,
            TlsTestResult test1Result,
            TlsTestResult test2Result,
            TlsTestResult test3Result,
            TlsTestResult test4Result,
            TlsTestResult test5Result,
            TlsTestResult test6Result,
            TlsTestResult test7Result,
            TlsTestResult test8Result,
            TlsTestResult test9Result,
            TlsTestResult test10Result,
            TlsTestResult test11Result,
            TlsTestResult test12Result,
            List<Certificate> certificates)
        {
            FailureCount = failureCount;
            Test1Result = test1Result;
            Test2Result = test2Result;
            Test3Result = test3Result;
            Test4Result = test4Result;
            Test5Result = test5Result;
            Test6Result = test6Result;
            Test7Result = test7Result;
            Test8Result = test8Result;
            Test9Result = test9Result;
            Test10Result = test10Result;
            Test11Result = test11Result;
            Test12Result = test12Result;
            Certificates = certificates ?? new List<Certificate>();
        }

        public int FailureCount { get; }
        public TlsTestResult Test1Result { get; }
        public TlsTestResult Test2Result { get; }
        public TlsTestResult Test3Result { get; }
        public TlsTestResult Test4Result { get; }
        public TlsTestResult Test5Result { get; }
        public TlsTestResult Test6Result { get; }
        public TlsTestResult Test7Result { get; }
        public TlsTestResult Test8Result { get; }
        public TlsTestResult Test9Result { get; }
        public TlsTestResult Test10Result { get; }
        public TlsTestResult Test11Result { get; }
        public TlsTestResult Test12Result { get; }
        public List<Certificate> Certificates { get; }

        //ignore failure count
        protected bool Equals(TlsTestResults other)
        {
            return Equals(Test1Result, other.Test1Result) && 
                   Equals(Test2Result, other.Test2Result) && 
                   Equals(Test3Result, other.Test3Result) && 
                   Equals(Test4Result, other.Test4Result) && 
                   Equals(Test5Result, other.Test5Result) && 
                   Equals(Test6Result, other.Test6Result) && 
                   Equals(Test7Result, other.Test7Result) && 
                   Equals(Test8Result, other.Test8Result) && 
                   Equals(Test9Result, other.Test9Result) && 
                   Equals(Test10Result, other.Test10Result) && 
                   Equals(Test11Result, other.Test11Result) && 
                   Equals(Test12Result, other.Test12Result) &&
                   Certificates.SequenceEqual(other.Certificates);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TlsTestResults) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Test1Result?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Test2Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test3Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test4Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test5Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test6Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test7Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test8Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test9Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test10Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test11Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Test12Result?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Certificates?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}