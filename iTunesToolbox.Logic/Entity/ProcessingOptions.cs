using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTunesToolbox.Logic.Entity
{
    public class ProcessingOptions
    {
        #region Members

        private bool _applyAllUpdates;
        private bool _logAllMessages;

        #endregion Members

        #region Constructor(s)

        public ProcessingOptions()
        {
            DisplayInformation = false;

            _applyAllUpdates = false;
            UpdateWindowsFiles = false;
            RemoveRatingOneTracks = false;

            _logAllMessages = false;
            LogInformationMessages = false;
            LogWarningMessages = false;
            LogErrorMessages = false;
        }

        #endregion Constructor(s)

        #region Properties

        public bool DisplayInformation { get; set; }

        public bool UpdateWindowsFiles { get; set; }
        public bool UpdateiTunesFiles { get; set; }
        public bool RenameFiles { get; set; }
        public bool RemoveRatingOneTracks { get; set; }

        public bool LogAllMessages
        {
            get
            {
                return _applyAllUpdates;
            }
            set
            {
                _logAllMessages = value;
                LogInformationMessages = value;
                LogWarningMessages = value;
                LogErrorMessages = value;
            }
        }
        public bool LogInformationMessages { get; set; }
        public bool LogWarningMessages { get; set; }
        public bool LogErrorMessages { get; set; }

        #endregion Properties
    }
}
