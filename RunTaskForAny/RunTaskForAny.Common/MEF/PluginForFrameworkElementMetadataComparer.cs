using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.MEF
{
    public class PluginForFrameworkElementMetadataComparer : IEqualityComparer<Lazy<IPluginForFrameworkElement, IMetadata>>
    {

        public bool Equals(Lazy<IPluginForFrameworkElement, IMetadata> x, Lazy<IPluginForFrameworkElement, IMetadata> y)
        {
            if (x.Metadata.Name == y.Metadata.Name)
            {
                return true;
            }
            else
            { 
                return false;
            }
        }


        public int GetHashCode(Lazy<IPluginForFrameworkElement, IMetadata> obj)
        {
            return obj.Metadata.Name.GetHashCode();
        }
    }
}
