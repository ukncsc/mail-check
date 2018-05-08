using System;
using Amazon.Lambda.Core;

namespace Dmarc.Common.Report.Evnts
{
    public class LambdaContext : ILambdaContext
    {
        public static ILambdaContext NonExpiringLambda = new LambdaContext(Guid.NewGuid().ToString(), null, string.Empty, string.Empty, 
            null, string.Empty, null, string.Empty, string.Empty, int.MaxValue, TimeSpan.MaxValue);

        public LambdaContext(string awsRequestId, IClientContext clientContext, string functionName, 
            string functionVersion, ICognitoIdentity identity, string invokedFunctionArn, ILambdaLogger logger, 
            string logGroupName, string logStreamName, int memoryLimitInMb, TimeSpan remainingTime)
        {
            AwsRequestId = awsRequestId;
            ClientContext = clientContext;
            FunctionName = functionName;
            FunctionVersion = functionVersion;
            Identity = identity;
            InvokedFunctionArn = invokedFunctionArn;
            Logger = logger;
            LogGroupName = logGroupName;
            LogStreamName = logStreamName;
            MemoryLimitInMB = memoryLimitInMb;
            RemainingTime = remainingTime;
        }

        public string AwsRequestId { get; }
        public IClientContext ClientContext { get; }
        public string FunctionName { get; }
        public string FunctionVersion { get; }
        public ICognitoIdentity Identity { get; }
        public string InvokedFunctionArn { get; }
        public ILambdaLogger Logger { get; }
        public string LogGroupName { get; }
        public string LogStreamName { get; }
        public int MemoryLimitInMB { get; }
        public TimeSpan RemainingTime { get; }
    }
}