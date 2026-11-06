using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialProfiles.Core.Domain.Presets;

public static class PresetCatalog
{
    public static IReadOnlyList<ThemePreset> All { get; } = new List<ThemePreset>
    {
        new DarkPreset(),
        new LightPreset()
    };
}
