using System;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore.Extensions
{
    public static class SPFeatureCollectionExtensions
    {
        public static bool IsActivated(this SPFeatureCollection features, Guid featureId)
        {
            return features.Any(f => f.DefinitionId.Equals(featureId));
        }

        public static void EnsureActivated(this SPFeatureCollection features, Guid featureId)
        {
            if (!IsActivated(features, featureId))
                features.Add(featureId);
        }

        public static void EnsureDeactivated(this SPFeatureCollection features, Guid featureId)
        {
            if (IsActivated(features, featureId))
                features.Remove(featureId);
        }
    }
}
