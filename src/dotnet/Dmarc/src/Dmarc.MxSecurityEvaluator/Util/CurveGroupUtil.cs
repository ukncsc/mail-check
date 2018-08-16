using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Util
{
    public static class CurveGroupUtil
    {
        public static string GetGroupName(this CurveGroup? curveGroup) =>
            curveGroup != null
                ? Enum.GetName(typeof(CurveGroup), curveGroup)
                : "";
    }
}