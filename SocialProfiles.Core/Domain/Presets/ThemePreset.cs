using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Core.Domain.Presets
{
    public abstract class ThemePreset
    {
        public abstract string Name { get; }

        public abstract Theme CreateTheme();
    }
}
