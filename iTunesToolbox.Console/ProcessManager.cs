using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesToolbox.Logic.Entity;
using iTunesToolbox.Logic.Common;

namespace iTunesToolbox.Logic
{
    public class ProcessManager
    {
        #region Members

        private ConsoleColor _defaultColour;
        private ProcessingOptions _options;

        private const ConsoleColor InformationColour = ConsoleColor.DarkGray;
        private const ConsoleColor WarningColour = ConsoleColor.DarkYellow;
        private const ConsoleColor ErrorColour = ConsoleColor.DarkRed;

        #endregion Members

        #region Constructor(s)

        public ProcessManager()
        {
            _defaultColour = System.Console.ForegroundColor;
        }

        public ProcessManager(ProcessingOptions options)
        {
            _options = options;
            _defaultColour = System.Console.ForegroundColor;
        }

        #endregion Constructor(s)

        #region Properties

        public ProcessingOptions Options 
        {
            get { return _options; }
            set { value = _options; }
        }

        public int ProcessedTracks { get; set; }
        public int UpdatedTracks { get; set; }
        public int RenamedTracks { get; set; }
        public int DeletedTracks { get; set; }
        public int UnratedTracks { get; set; }
        public int SkippedTracks { get; set; }
        public int ErrorTracks { get; set; }

        #endregion Properties

        #region Methods

        public void Execute()
        {
            var manager = new iTunesManager();
            if (!manager.Load()) return;

            System.Console.WriteLine(string.Format("iTunes Library Loaded, Processing {0} tracks", manager.TotalTracks));
            System.Console.WriteLine(string.Empty);

            if (_options.UpdateWindowsFiles) 
                System.Console.WriteLine("Updating Windows Files from iTunes ...");
            else if (_options.UpdateiTunesFiles)
                System.Console.WriteLine("Updating iTunes from Windows Files ...");

            System.Console.WriteLine(string.Empty);

            while (manager.NextTrack()) {
                ProcessTrack(manager);
            }

            decimal percentCompleted = 0;
            decimal ratedTracks = Convert.ToDecimal(manager.TotalTracks) - Convert.ToDecimal(UnratedTracks);
            if (manager.TotalTracks > 0) percentCompleted = ratedTracks / Convert.ToDecimal(manager.TotalTracks);
            percentCompleted = Math.Round(percentCompleted * 100, 0);

            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(string.Format("\tTotal Tracks:{0}\tProcessed:{1}\tErrored:{2}\tSkipped:{3}",
                manager.TotalTracks, ProcessedTracks, ErrorTracks, SkippedTracks));
            System.Console.WriteLine(string.Format("\tUpdated:{0}\tRenamed:{1}\tDeleted:{2}\tUnrated:{3}",
                UpdatedTracks, RenamedTracks, DeletedTracks, UnratedTracks));

            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(string.Format("{0}% of iTunes Library Rated", percentCompleted));
        }

        private void ProcessTrack(iTunesManager manager)
        {
            iTunesFile iTunesTrack = null;

            try
            {
                iTunesTrack = manager.CurrentTrack();
                if (!IsTrackValid(manager, iTunesTrack)) return;

                if (iTunesTrack.Rating == (int)iTunesFile.RatingConstant.One)
                {
                    if (_options.RemoveRatingOneTracks)
                        DeleteTrackAndFile(manager, iTunesTrack);

                    return;
                }

                var windowsFile = new WindowsFile(iTunesTrack.Location);

                if (_options.UpdateWindowsFiles)
                    UpdateWindowsFile(iTunesTrack, windowsFile);
                else if (_options.UpdateiTunesFiles)
                    UpdateiTunesTrack(iTunesTrack, windowsFile);

                if (_options.RenameFiles)
                    RenameWindowsFiles(iTunesTrack, windowsFile);

                if (_options.DisplayInformation)
                    DisplayFileInformaiton(iTunesTrack, windowsFile);

                if (iTunesTrack.Rating == (int)iTunesFile.RatingConstant.Unrated) 
                    UnratedTracks++;

                ProcessedTracks++;
            }
            catch (Exception exception)
            {
                if (_options.LogErrorMessages)
                {
                    var location = "Unknown Locaiton";
                    if (null != iTunesTrack) location = iTunesTrack.Location;

                    System.Console.ForegroundColor = ErrorColour;
                    System.Console.WriteLine(string.Format("{0}\t{1}", exception.Message, location));
                    System.Console.ForegroundColor = _defaultColour;
                }
                
                ErrorTracks++;
            }
        }

        private void DeleteTrackAndFile(iTunesManager manager, iTunesFile iTunesTrack)
        {
            var location = iTunesTrack.Location;

            var deleted = !System.IO.File.Exists(location);
            if (!deleted) deleted = DeleteWindowsFile(location);

            if (!deleted) return;
            iTunesTrack.RemoveTrack();

            System.Console.WriteLine(string.Format("Deleted:\t{0}", location));
            manager.CurrentTrackIndex--;
            DeletedTracks++;
        }

        private bool DeleteWindowsFile(string location)
        {
            try
            {
                System.IO.File.Delete(location);
                return true;
            }
            catch (Exception exception)
            {
                if (_options.LogErrorMessages)
                {
                    System.Console.ForegroundColor = ErrorColour;
                    System.Console.WriteLine(string.Format("Unable to delete:\t{0}\r\n{1}", location, exception.Message));
                    System.Console.ForegroundColor = _defaultColour;
                }
                
                return false;
            }
        }

        private bool IsTrackValid(iTunesManager manager, iTunesFile iTunesTrack)
        {
            if (null == iTunesTrack)
            {
                SkippedTracks++;
                InvalidIndex(manager.CurrentTrackIndex);
                return false;
            }

            if (!iTunesTrack.IsValidKind())
            {
                SkippedTracks++;
                InvalidTrackKind(iTunesTrack);
                return false;
            }

            if (!iTunesTrack.HasLocation())
            {
                MissingLocation(manager, iTunesTrack);
                return false;
            }

            if (!iTunesTrack.IsValidLocation())
            {
                SkippedTracks++;
                InvalidLocation(manager, iTunesTrack);
                return false;
            }

            if (!iTunesTrack.IsValidExtention())
            {
                SkippedTracks++;
                InvalidExtention(iTunesTrack);
                return false;
            }

            return true;
        }
        
        private void UpdateWindowsFile(iTunesFile iTunesTrack, WindowsFile windowsFile)
        {
            //windowsFile.OriginalTitle = windowsFile.Title;
            //windowsFile.OriginalArtist = windowsFile.Artist;
            //windowsFile.OriginalAlbum = windowsFile.Album;
            windowsFile.OriginalRating = windowsFile.Rating;

            var newWindowsRating = RatingConverter.AsWindowsRating(iTunesTrack.Rating);
            if (windowsFile.OriginalRating == newWindowsRating)
                return;
            
            //windowsFile.Title = iTunesTrack.Title;
            //windowsFile.Artist = iTunesTrack.Artist;
            //windowsFile.Album = iTunesTrack.Album;
            windowsFile.Rating = newWindowsRating;

            ValidateFileUpdate(newWindowsRating, iTunesTrack, windowsFile);
        }

        private void UpdateiTunesTrack(iTunesFile iTunesTrack, WindowsFile windowsFile)
        {
            //iTunesTrack.OriginalTitle = iTunesTrack.Title;
            //iTunesTrack.OriginalAlbum = iTunesTrack.Album;
            //iTunesTrack.OriginalArtist = iTunesTrack.Artist;
            iTunesTrack.OriginalRating = iTunesTrack.Rating;

            var newiTunesRating = RatingConverter.AsiTunesRating(windowsFile.Rating);
            if (iTunesTrack.OriginalRating == newiTunesRating)
                return;

            //iTunesTrack.Title = windowsFile.Title;
            //iTunesTrack.Album = windowsFile.Album;
            //iTunesTrack.Artist = windowsFile.Artist;
            iTunesTrack.Rating = newiTunesRating;
            ValidateTrackUpdate(newiTunesRating, iTunesTrack, windowsFile);
        }

        private void ValidateFileUpdate(int expectedRating, iTunesFile iTunesTrack, WindowsFile windowsFile)
        {
            var actualRating = windowsFile.Rating;
            if (actualRating == expectedRating)
            {
                System.Console.WriteLine(
                    string.Format("Updated:{0}=>{1}\tSource:{2}\t{3}[{4}] : {5}",
                        windowsFile.OriginalSimpleRating, windowsFile.SimpleRating, iTunesTrack.SimpleRating,
                        iTunesTrack.Artist, iTunesTrack.Album, iTunesTrack.Title));

                UpdatedTracks++;
            }
            else
            {
                var colour = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine(
                    string.Format("Failure:{0}=>{1}\tSource:{2}\t{3}[{4}] : {5}",
                        windowsFile.OriginalSimpleRating, windowsFile.SimpleRating, iTunesTrack.SimpleRating,
                        iTunesTrack.Artist, iTunesTrack.Album, iTunesTrack.Title));
                System.Console.ForegroundColor = colour;

                ErrorTracks++;
            }
        }

        private void ValidateTrackUpdate(int expectedRating, iTunesFile iTunesTrack, WindowsFile windowsFile)
        {
            var actualRating = iTunesTrack.Rating;
            if (actualRating == expectedRating)
            {
                System.Console.WriteLine(
                    string.Format("Updated:{0}=>{1}\tSource:{2}\t{3}[{4}] : {5}",
                        iTunesTrack.OriginalSimpleRating, iTunesTrack.SimpleRating, windowsFile.SimpleRating,
                        iTunesTrack.Artist, iTunesTrack.Album, iTunesTrack.Title));

                UpdatedTracks++;
            }  else {
                var colour = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine(
                    string.Format("Failure:{0}=>{1}\tSource:{2}\t{3}[{4}] : {5}",
                        iTunesTrack.OriginalSimpleRating, iTunesTrack.SimpleRating, windowsFile.SimpleRating,
                        iTunesTrack.Artist, iTunesTrack.Album, iTunesTrack.Title));
                System.Console.ForegroundColor = colour;

                ErrorTracks++;
            }
        }

        private void RenameWindowsFiles(iTunesFile iTunesTrack, WindowsFile windowsFile)
        {
            var originalLocation = System.IO.Path.GetFileName(iTunesTrack.Location);
            iTunesTrack.Location = windowsFile.UpdateFileName();
            var updatedLocation = System.IO.Path.GetFileName(iTunesTrack.Location);

            if (updatedLocation != originalLocation)
            {
                RenamedTracks++;
                Console.WriteLine(string.Format("Renamed: {0} => {1}", originalLocation, updatedLocation));
            }
        }

        #region Console Messages

        private void DisplayFileInformaiton(iTunesFile iTunesTrack, WindowsFile windowsFile)
        {
            System.Console.WriteLine(
                    string.Format("Windows:{0}\tiTunes:{1}\t{2} [{3}] - {4}",
                        windowsFile.SimpleRating, iTunesTrack.SimpleRating,
                        iTunesTrack.Artist, iTunesTrack.Album, iTunesTrack.Title));
        }

        private void InvalidIndex(int currentTrackIndex)
        {
            if (!_options.LogWarningMessages)
                return;

            System.Console.ForegroundColor = WarningColour;
            System.Console.WriteLine(string.Format("Unable to load iTunes Track at index:\t{0}", currentTrackIndex));
            System.Console.ForegroundColor = _defaultColour;
        }

        private void InvalidTrackKind(iTunesFile iTunesTrack)
        {
            if (!_options.LogWarningMessages)
                return;

            System.Console.ForegroundColor = WarningColour;
            System.Console.WriteLine(string.Format("Skipping\tInvalid Track Kind:{0}", iTunesTrack.Kind));
            System.Console.ForegroundColor = _defaultColour;
        }

        private void MissingLocation(iTunesManager manager, iTunesFile iTunesTrack)
        {
            if (!_options.LogInformationMessages)
                return;

            System.Console.ForegroundColor = InformationColour;
            System.Console.WriteLine(string.Format("Removed Track from iTunes (Invalid Location)\t{0}[{1}] : {2}",
                iTunesTrack.Artist, iTunesTrack.Album, iTunesTrack.Title));
            System.Console.ForegroundColor = _defaultColour;

            iTunesTrack.RemoveTrack();
            manager.CurrentTrackIndex--;
        }

        private void InvalidLocation(iTunesManager manager, iTunesFile iTunesTrack)
        {
            if (!_options.LogInformationMessages)
                return;

            System.Console.ForegroundColor = InformationColour;
            System.Console.WriteLine(string.Format("Removed Track from iTunes (Invalid Location)\t{0}", iTunesTrack.Location));
            System.Console.ForegroundColor = _defaultColour;

            iTunesTrack.RemoveTrack();
            manager.CurrentTrackIndex--;
        }

        private void InvalidExtention(iTunesFile iTunesTrack)
        {
            if (!_options.LogWarningMessages)
                return;

            System.Console.ForegroundColor = WarningColour;
            System.Console.WriteLine(string.Format("Skipping\tInvalid Extention : {0}", iTunesTrack.Location));
            System.Console.ForegroundColor = _defaultColour;
        }
        
        #endregion Console Messages

        #endregion Methods
    }
}
