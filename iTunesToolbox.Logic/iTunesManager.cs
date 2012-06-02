using System;
using iTunesLib;
using iTunesToolbox.Logic.Entity;

namespace iTunesToolbox.Logic
{
    public class iTunesManager
    {
        #region Members
        
        private iTunesAppClass _appClass;
        private IITLibraryPlaylist _mainLibrary;
        private IITTrackCollection _tracks;

        #endregion Members

        #region Constructor(s)
        
        public iTunesManager()
        {
            _appClass = new iTunesAppClass();
        }

        #endregion Constructor(s)

        #region Properties
        
        public int CurrentTrackIndex { get; set; }
        public int TotalTracks { get { return _tracks.Count; } }
        public IITTrackCollection Tracks { get { return _tracks; } }
        
        #endregion Properties

        #region Methods

        public bool Load()
        {
            try
            {
                _mainLibrary = _appClass.LibraryPlaylist;
                _tracks = _mainLibrary.Tracks;

                CurrentTrackIndex = 0;
                return true;
            }
            catch (Exception exception)
            {
                //TODO: Log exception
                throw exception;
            }
        }

        public iTunesFile CurrentTrack()
        {
            if ((CurrentTrackIndex < 1) || (CurrentTrackIndex > Tracks.Count))
                CurrentTrackIndex = 1;

            var track = new iTunesFile();
            return track.Load(CurrentTrackIndex, Tracks) ? track : null;
        }

        public bool NextTrack()
        {
            CurrentTrackIndex++;
            return (CurrentTrackIndex >= 1) && (CurrentTrackIndex <= _tracks.Count);
        }

        #endregion Methods
    }
}
