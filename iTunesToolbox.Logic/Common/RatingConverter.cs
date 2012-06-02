using iTunesToolbox.Logic.Entity;

namespace iTunesToolbox.Logic.Common
{
    public static class RatingConverter
    {
        public static int AsWindowsRating(int iTunesRating)
        {
            switch (iTunesRating)
            {
                case (int)iTunesFile.RatingConstant.One:
                    return (int)WindowsFile.RatingConstant.One;
                case (int)iTunesFile.RatingConstant.Two:
                    return (int)WindowsFile.RatingConstant.Two;
                case (int)iTunesFile.RatingConstant.Three:
                    return (int)WindowsFile.RatingConstant.Three;
                case (int)iTunesFile.RatingConstant.Four:
                    return (int)WindowsFile.RatingConstant.Four;
                case (int)iTunesFile.RatingConstant.Five:
                    return (int)WindowsFile.RatingConstant.Five;
                default:
                    return (int)WindowsFile.RatingConstant.Unrated;
            }
        }

        public static int AsiTunesRating(int windowsRating)
        {
            switch (windowsRating)
            {
                case (int)WindowsFile.RatingConstant.One:
                    return (int)iTunesFile.RatingConstant.One;
                case (int)WindowsFile.RatingConstant.Two:
                    return (int)iTunesFile.RatingConstant.Two;
                case (int)WindowsFile.RatingConstant.Three:
                    return (int)iTunesFile.RatingConstant.Three;
                case (int)WindowsFile.RatingConstant.Four:
                    return (int)iTunesFile.RatingConstant.Four;
                case (int)WindowsFile.RatingConstant.Five:
                    return (int)iTunesFile.RatingConstant.Five;
                default:
                    return (int)iTunesFile.RatingConstant.Unrated;
            }
        }
    }
}
