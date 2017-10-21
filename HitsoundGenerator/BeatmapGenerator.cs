using BeatmapLib;
using BeatmapLib.Events;
using BeatmapLib.HitObjects;
using HitsoundGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitsoundGenerator
{
    public class BeatmapGenerator
    {
        const float eps = 1;

        private Beatmap baseMap;
        public Beatmap generatedMap;

        public BeatmapGenerator(Beatmap baseMap)
        {
            this.baseMap = baseMap;
            generatedMap = new Beatmap(baseMap);
        }

        public Beatmap exportBeatmap()
        {
            return generatedMap;
        }

        private void addHitsound(ConfiguredHitsound pattern)
        {
            int objectIndex = 0;
            int currentHSindex = 0;
            float currentOffset = (float)pattern.startOffset;

            var timingPoint = generatedMap.TimingPoints[0];
            if (timingPoint.Time > currentOffset)
                currentOffset = timingPoint.Time;

            EffectType getHS()
            {
                if (currentHSindex < pattern.hs.Count)
                    return pattern.hs[currentHSindex];
                currentHSindex = currentHSindex % pattern.hs.Count;
                return getHS();
            }

            while (currentOffset < pattern.endOffset)
            {
                int newObjectIndex = generatedMap.FindObjectOwningStartTime(currentOffset, objectIndex);

                timingPoint = generatedMap.TimingPointByTime(currentOffset, true);

                if (newObjectIndex == -1)
                {
                    currentOffset += timingPoint.BpmDelay / pattern.period;
                    ++currentHSindex;
                    continue;
                }

                objectIndex = newObjectIndex;
                var currentObject = generatedMap.HitObjects[objectIndex];
                if (currentObject.Type.HasFlag(HitObjectType.Circle))
                    currentOffset = currentObject.StartTime;

                if (currentObject.Type.HasFlag(HitObjectType.Circle))
                    currentObject.Effect |= getHS();

                if (currentObject.Type.HasFlag(HitObjectType.Spinner))
                {
                    var spinner = (SpinnerObject)currentObject;
                    if (BeatmapUtils.checkObjectCloseByTime(spinner.EndTime, currentOffset))
                    {
                        spinner.Effect |= getHS();
                    }
                }

                if (currentObject.Type.HasFlag(HitObjectType.Slider))
                {
                    var slider = (SliderObject)currentObject;
                    int sliderSegmentIndex = BeatmapUtils.FindSliderRepeatPointOwningTime(slider, currentOffset);
                    if (sliderSegmentIndex >= 0)
                        slider.effects[sliderSegmentIndex] |= getHS();
                }

                currentOffset += timingPoint.BpmDelay / pattern.period;
                ++currentHSindex;
            }

        }

        public void addHitsounds(IEnumerable<ConfiguredHitsound> patterns)
        {
            foreach (var p in patterns)
            {
                addHitsound(p);
            }
        }

        public void addBreak(BreakEvent b)
        {
            generatedMap.Events.Add(b);
        }

        public void clearHitsounds()
        {
            foreach (var obj in this.generatedMap.HitObjects)
            {
                obj.Effect = 0;

                if (obj.Type.HasFlag(HitObjectType.Slider))
                {
                    ((SliderObject)obj).effects = Enumerable.Repeat((EffectType)0, ((SliderObject)obj).effects.Count).ToList();
                }
            }
        }
    }
}
