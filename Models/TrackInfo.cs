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
    class TrackInfo
    {
        private string _author;
        private string _title;
        private string _album;
        private string _cover;
        private int _duration;
        private string _durationHuman;
        private string _url;
        private string _id;
        private bool _isVideo;
        private bool _isAdvertisement;
        private bool _inLibrary;

        public string Author => _author;
        public string Title => _title;
        public string Album => _album;
        public string Cover => _cover;
        public int Duration => _duration;
        public string DurationHuman => _durationHuman;
        public string Url => _url;
        public string Id => _id;
        public bool IsVideo => _isVideo;
        public bool IsAdvertisement => _isAdvertisement;
        public bool InLibrary => _inLibrary;

        public TrackInfo()
        {
            _author = "";
            _title = "";
            _album = "";
            _cover = "";
            _duration = 0;
            _durationHuman = "0:00";
            _url = "";
            _id = "";
            _isVideo = false;
            _isAdvertisement = false;
            _inLibrary = false;
        }
        public void FromJson(dynamic json)
        {
            try
            {
                var parsedJson = JObject.Parse(json.ToString());
                parsedJson = parsedJson["track"];

                _author = parsedJson["author"];
                _title = parsedJson["title"];
                _album = parsedJson["album"];
                _cover = parsedJson["cover"];
                _duration = parsedJson["duration"];
                _durationHuman = parsedJson["durationHuman"];
                _url = parsedJson["url"];
                _id = parsedJson["id"];
                _isVideo = parsedJson["isVideo"];
                _isAdvertisement = parsedJson["isAdvertisement"];
                _inLibrary = parsedJson["inLibrary"];
            }
            catch { }
        }
    }
}
