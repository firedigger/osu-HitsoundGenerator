using BeatmapLib;
using HitsoundGenerator;
using System.Collections.Generic;

namespace HitsoundGenerator
{
    public class ConfiguredHitsound
    {
        public List<EffectType> hs;
        public double startOffset;
        public double endOffset;
        public int period;

        public ConfiguredHitsound(string meta, double startOffset, double endOffset, int period = 2)
        {
            hs = new List<EffectType>();

            foreach(var c in meta)
            {
                switch(c)
                {
                    case 'W': hs.Add(EffectType.Whistle); break;
                    case 'F': hs.Add(EffectType.Finish); break;
                    case 'C': hs.Add(EffectType.Clap); break;
                    default: hs.Add(EffectType.None); break;
                }
            }

            this.startOffset = startOffset;
            this.endOffset = endOffset;
            this.period = period;
        }

        private static string shortEffectString(EffectType hs)
        {
            switch (hs)
            {
                case EffectType.Whistle: return "W";
                case EffectType.Clap: return "C";
                case EffectType.Finish: return "F";
                default: return "_";
            }
        }

        public override string ToString()
        {
            string s = "";

            foreach(var h in hs)
                s += shortEffectString(h);

            return s;
        }

    }
}