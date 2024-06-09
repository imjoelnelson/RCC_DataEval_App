using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class ScatterSelectItem : INotifyPropertyChanged
    {
        /// <summary>
        /// FileName for the RCC; displayed in the sample vs sample DGV
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The RCC for the sample in question
        /// </summary>
        public Rcc ThisRcc { get; set; }
        /// <summary>
        /// Bool indicating if RCC/sample is selected for the x axis of the scatterplot
        /// </summary>
        public bool X
        {
            get { return _X; }
            set
            {
                if(_X != value)
                {
                    _X = value;
                    NotifyPropertyChanged();
                }

            }
        }
        private bool _X;

        /// <summary>
        /// Bool indicating if the RCC/sample is selcted for the y axis of the scatterplot
        /// </summary>
        public bool Y
        {
            get { return _Y; }
            set
            {
                if(_Y != value)
                {
                    _Y = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _Y;

        
        public ScatterSelectItem(Rcc rcc)
        {
            ThisRcc = rcc;
            Name = ThisRcc.FileName;
            X = false;
            Y = false;
        }

        // NotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
