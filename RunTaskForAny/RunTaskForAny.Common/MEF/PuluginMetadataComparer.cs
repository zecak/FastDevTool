using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.MEF
{
    public class PuluginMetadataComparer : IEqualityComparer<Lazy<IPlugin, IMetadata>>
    {

        public bool Equals(Lazy<IPlugin, IMetadata> x, Lazy<IPlugin, IMetadata> y)
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


        public int GetHashCode(Lazy<IPlugin, IMetadata> obj)
        {
            return obj.Metadata.Name.GetHashCode();
        }
    }
}
