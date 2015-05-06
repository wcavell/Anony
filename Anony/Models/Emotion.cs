using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anony.Models
{
    class Emotion
    {
        public string Img { get; set; }
        public string Name { get; set; }
    }
    class Emoji
    {
        public ObservableCollection<Emotion> Emotions { get; set; }
        public string Name { get; set; }
    }
}
