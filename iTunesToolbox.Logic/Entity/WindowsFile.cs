using System;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace iTunesToolbox.Logic.Entity
{
    public class WindowsFile
    {
        #region Members
        
        private string _fileLocation;
        private ShellObject _shellObject = null;

        public const string RenameFormat = "{0} [{1}] - {2}";

        public enum RatingConstant { Unrated = 0, One = 1, Two = 25, Three = 50, Four = 75, Five = 99 }
        public enum SimpleRatingConstant { Unrated = 0, One = 1, Two = 2, Three = 3, Four = 4, Five = 5 }

        #endregion Members

        #region Constructor(s)
        
        public WindowsFile(string fileLocation)
        {
            _fileLocation = fileLocation;
            _shellObject = ShellObject.FromParsingName(_fileLocation);
        }

        #endregion Constructor(s)

        #region Properties

        public string OriginalTitle { get; set; }
        public string Title
        {
            get { return ReadStringProperty(SystemProperties.System.Title); }
            set { WriteProperty(OriginalTitle, value, SystemProperties.System.Title); }
        }

        public string OriginalArtist { get; set; }
        public string Artist
        {
            get { return GetArtistName(); }
            set { SetArtistName(value); }
        }

        public string OriginalAlbum { get; set; }
        public string Album
        {
            get { return GetAlbumName(); }
            set { SetAlbumName(value); }
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
            get { return ReadIntProperty(SystemProperties.System.Rating); }
            set { WriteProperty(OriginalTitle, value, SystemProperties.System.Rating); }
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
            get { return ReadDateProperty(SystemProperties.System.DateModified); }
        }

        #endregion Properties

        #region Methods

        public bool Load()
        {
            var shellObject = ShellObject.FromParsingName(_fileLocation);
            if (null == shellObject) return false;
            if (null == shellObject.Properties) return false;

            return true;
        }

        public string UpdateFileName()
        {
            var path = System.IO.Path.GetDirectoryName(_fileLocation);
            var extention = System.IO.Path.GetExtension(_fileLocation);

            var fileName = string.Format(RenameFormat, Artist, Album, Title);
            var updatedLocation = string.Format(@"{0}\{1}{2}", path, fileName, extention);

            if (!_fileLocation.Equals(updatedLocation))
                System.IO.File.Move(_fileLocation, updatedLocation);

            return updatedLocation;
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

        #region Windows API Calls

        private string ReadStringProperty(PropertyKey key)
        {
            if (null == _shellObject)
                return string.Empty;

            var shellProperty = _shellObject.Properties.GetProperty(key);
            if (null == shellProperty)
                return string.Empty;

            var obj = shellProperty.ValueAsObject;
            return (null != obj) ? obj.ToString() : string.Empty;
        }

        private int ReadIntProperty(PropertyKey key)
        {
            var value = ReadStringProperty(key);

            int intValue;
            if (!int.TryParse(value, out intValue))
                intValue = 0;

            return intValue;
        }

        private DateTime ReadDateProperty(PropertyKey key)
        {
            var value = ReadStringProperty(key);

            DateTime dateValue;
            if (!DateTime.TryParse(value, out dateValue))
                dateValue = DateTime.MinValue;

            return dateValue;
        }

        private void WriteProperty(object originalValue, object value, PropertyKey key)
        {
            if (originalValue == value)
                return;

            using (var writer = _shellObject.Properties.GetPropertyWriter())
            {
                writer.WriteProperty(key, value);
                writer.Close();
            }
        }

        #endregion Windows API Calls

        #region TagLib-Sharp Calls

        private string GetAlbumName()
        {
            using (var file = TagLib.File.Create(_fileLocation))
            {
                return file.Tag.Album;
            }
        }

        private void SetAlbumName(string value)
        {
            if (OriginalAlbum == value)
                return;

            using (var file = TagLib.File.Create(_fileLocation))
            {
                file.Tag.Album = value;
                file.Save();
            }
        }

        private string GetArtistName()
        {
            using (var file = TagLib.File.Create(_fileLocation))
            {
                return file.Tag.FirstPerformer;
            }
        }

        private void SetArtistName(string value)
        {
            if (OriginalArtist == value)
                return;

            using (var file = TagLib.File.Create(_fileLocation))
            {
                file.Tag.Performers = new string[] { value };
                file.Save();
            }
        }

        #endregion TagLib-Sharp Calls

        #endregion Methods
    }
}
