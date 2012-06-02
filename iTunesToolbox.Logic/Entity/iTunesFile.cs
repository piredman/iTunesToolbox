using System;
using System.Collections.Generic;
using iTunesLib;

namespace iTunesToolbox.Logic.Entity
{
    public class iTunesFile
    {
        #region Members
        
        private IITFileOrCDTrack _file;
        List<string> extentionsToIgnore = new List<string> { ".aa" };

        public enum RatingConstant { Unrated = 0, One = 20, Two = 40, Three = 60, Four = 80, Five = 100 }
        public enum SimpleRatingConstant { Unrated = 0, One = 1, Two = 2, Three = 3, Four = 4, Five = 5 }

        #endregion Members

        #region Constructor(s)

        public iTunesFile()
        {
            _file = null;
        }

        #endregion Constructor(s)

        #region Properties

        public string Location 
        { 
            get
            {
                return (null == _file) ? string.Empty : _file.Location;
            }
            set
            {
                if (null == _file)
                    return;

                _file.Location = value;
            }
        }

        public string Kind
        {
            get
            {
                return (null == _file) ? string.Empty : _file.KindAsString;
            }
        }

        public string OriginalTitle { get; set; }
        public string Title
        {
            get
            {
                return (null == _file) ? string.Empty : _file.Name;
            }
            set
            {
                if (null == _file)
                    return;

                _file.Name = value;
            }
        }

        public string OriginalArtist { get; set; }
        public string Artist
        {
            get
            {
                return (null == _file) ? string.Empty : _file.Artist;
            }
            set
            {
                if (null == _file)
                    return;

                _file.Artist = value;
            }
        }

        public string OriginalAlbum { get; set; }
        public string Album
        {
            get
            {
                return (null == _file) ? string.Empty : _file.Album;
            }
            set
            {
                if (null == _file)
                    return;

                _file.Album = value;
            }
        }

        public int OriginalRating { get; set; }
        public int OriginalSimpleRating
        {
            get
            {
                return GetSimpleRating(OriginalRating);
            }
        }

        public int Rating
        {
            get
            {
                return (null == _file) ? 0 : _file.Rating;
            }
            set
            {
                if (null == _file) 
                    return;

                if (OriginalRating == value)
                    return;

                _file.Rating = value;
            }
        }
        public int SimpleRating
        {
            get
            {
                return GetSimpleRating(Rating);
            }
        }

        public DateTime LastModified
        {
            get
            {
                return _file.ModificationDate;
            }
        }

        #endregion Properties

        #region Methods

        public bool Load(int index, IITTrackCollection tracks)
        {
            // only work with files
            _file = tracks[index] as IITFileOrCDTrack;
            return null != _file;
        }
        
        public bool IsValidKind()
        {
            // is this a file track?
            return (_file.Kind == ITTrackKind.ITTrackKindFile);
        }

        public bool HasLocation()
        {
            // Does the track have a valid location
            return (null != _file.Location);
        }

        public bool IsValidLocation()
        {
            // Get a reference to the tracks file
            return System.IO.File.Exists(_file.Location);
        }

        public bool IsValidExtention()
        {
            var extension = System.IO.Path.GetExtension(_file.Location);
            return !extentionsToIgnore.Contains(extension);
        }

        public void RemoveTrack()
        {
            _file.Delete();
        }

        private int GetSimpleRating(int rating)
        {
            switch (rating)
            {
                case (int)RatingConstant.One:
                    return (int)SimpleRatingConstant.One;
                case (int)RatingConstant.Two:
                    return (int)SimpleRatingConstant.Two;
                case (int)RatingConstant.Three:
                    return (int)SimpleRatingConstant.Three;
                case (int)RatingConstant.Four:
                    return (int)SimpleRatingConstant.Four;
                case (int)RatingConstant.Five:
                    return (int)SimpleRatingConstant.Five;
                default:
                    return (int)SimpleRatingConstant.Unrated;
            }
        }

        #endregion Methods
    }
}
