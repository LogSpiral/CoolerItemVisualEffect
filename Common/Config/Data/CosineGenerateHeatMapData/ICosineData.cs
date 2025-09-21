using System;
using System.Collections.Generic;

namespace CoolerItemVisualEffect.Common.Config.Data.CosineGenerateHeatMapData;

public interface ICosineData
{
    IReadOnlyList<CosineInfo> Cosines { get; }
    Func<float, CosineInfo, Color>[] LineColorMethods { get; }
    Color[] LineColors { get; }
    Color GetValue(float t);
}
