using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.Serialization.Json;

using Newtonsoft.Json.Linq;

namespace YouTubeMusicDesktopWidget.Models
{
    class PlayerInfo
    {
        private bool _isPaused;
        private int _volumePercent;
        private int _seekbarCurrentPosition;
        private string _seekbarCurrentPositionHuman;
        private string _likeStatus;
        private string _repeatType;

        public bool IsPaused => _isPaused;
        public int VolumePercent => _volumePercent;
        public int SeekbarCurrentPosition => _seekbarCurrentPosition;
        public string SeekbarCurrentPositionHuman => _seekbarCurrentPositionHuman;
        public string LikeStatus => _likeStatus;
        public string RepeatType => _repeatType;

        public PlayerInfo()
        {
            _isPaused = true;
            _volumePercent = 0;
            _seekbarCurrentPosition = 0;
            _seekbarCurrentPositionHuman = "0:00";
            _likeStatus = "INDIFFERENT";
            _repeatType = "";
        }
        public void FromJson(dynamic json)
        {
            try
            {
                var parsedJson = JObject.Parse(json.ToString());
                parsedJson = parsedJson["player"];

                _isPaused = parsedJson["isPaused"];
                _volumePercent = parsedJson["volumePercent"];
                _seekbarCurrentPosition = parsedJson["seekbarCurrentPosition"];
                _seekbarCurrentPositionHuman = parsedJson["seekbarCurrentPositionHuman"];
                _likeStatus = parsedJson["likeStatus"];
                _repeatType = parsedJson["repeatType"];
            }
            catch { }
        }
    }
}
