using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Dmarc.DomainStatus.Api.Util
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NoParametersConstraint : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return !routeContext.HttpContext.Request.Query.Any();
        }
    }
}