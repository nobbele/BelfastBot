using System.IO;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;

namespace OsuApi
{
    public class FakeWorkingBeatmap : WorkingBeatmap
    {
        protected Stream BeatmapStream;

        public FakeWorkingBeatmap(Stream beatmapStream, BeatmapInfo beatmapInfo) 
            : base(beatmapInfo, null) 
        {
            BeatmapStream = beatmapStream;
        }

        protected override Texture GetBackground()
        {
            throw new System.NotImplementedException();
        }

        protected override IBeatmap GetBeatmap()
        {
            try
            {
                using (var stream = new LineBufferedReader(BeatmapStream))
                    return Decoder.GetDecoder<osu.Game.Beatmaps.Beatmap>(stream).Decode(stream);
            }
            catch
            {
                return null;
            }
        }

        protected override Track GetTrack()
        {
            throw new System.NotImplementedException();
        }

        protected override VideoSprite GetVideo()
        {
            throw new System.NotImplementedException();
        }
    }
}