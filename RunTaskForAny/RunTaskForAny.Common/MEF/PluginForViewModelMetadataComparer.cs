using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.MEF
{
    public class PluginForViewModelMetadataComparer : IEqualityComparer<Lazy<IPluginForViewModel, IMetadata>>
    {

        public bool Equals(Lazy<IPluginForViewModel, IMetadata> x, Lazy<IPluginForViewModel, IMetadata> y)
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


        public int GetHashCode(Lazy<IPluginForViewModel, IMetadata> obj)
        {
            return obj.Metadata.Name.GetHashCode();
        }
    }
}
