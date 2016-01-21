﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyPI.LazyObjects
{
    public class AFEventFrame : BaseObject
    {
        private DateTimeOffset _StartTime;
        private DateTimeOffset _EndTime;
        private Lazy<AFElementTemplate> _Template;
        private Lazy<ObservableCollection<AFEventFrame>> _EventFrames;
        private Lazy<ObservableCollection<AFAttribute>> _Attributes;
        private ObservableCollection<string> _CategoryNames;
        private static IAFEventFrame _EventFrameLoader;

        #region "Properties"
        public DateTimeOffset StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                _StartTime = value;
            }
        }

        public DateTimeOffset EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                _EndTime = value;
            }
        }

        public AFElementTemplate Template
        {
            get
            {
                return _Template.Value;
            }
        }

        public IEnumerable<string> CategoryNames
        {
            get
            {
                return _CategoryNames;
            }
            set
            {
                _CategoryNames = value;
            }
        }
        #endregion

        #region "Constructors"
            private AFEventFrame(string ID, string Name, string Description, string Path) 
                : base(ID, Name, Description, Path)
            {
                Initialize();
            }

            private void Initialize()
            {
                //_Template = new Lazy<AFElementTemplate>(() => { 

                //}, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

                _EventFrames = new Lazy<ObservableCollection<AFEventFrame>>(() => {
                    var frames = _EventFrameLoader.GetChildFrames(_ID, SearchMode.None, "*-8d", "*", "*", "*", "*", "*", "*", false, "Name", "Ascending", 0, 1000);

                    ObservableCollection<AFEventFrame> obsList = new ObservableCollection<AFEventFrame>(EventFrameFactory.CreateInstanceList(frames));
                    obsList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ChildrenChanged);
                    return obsList;
                }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

                _Attributes = new Lazy<ObservableCollection<AFAttribute>>(() => { 
                    //TODO: Investigate what could be done better with search properties
                    var attrs = _EventFrameLoader.GetAttributes(this._ID, "*", "*", "*", "*", false, "Name", "Ascending", 0, false, false, 1000);
                    ObservableCollection<AFAttribute> obsList = new ObservableCollection<AFAttribute>(attr);
                }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            }
        #endregion

        private void ChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //TODO: Implement
        }

        public class EventFrameFactory
        {
            public static AFEventFrame CreateInstance(string ID, string Name, string Description, string Path)
            {
                return new AFEventFrame(ID, Name, Description, Path);
            }

            public static List<AFEventFrame> CreateInstanceList(IEnumerable<BaseObject> frames)
            {
                List<AFEventFrame> results = new List<AFEventFrame>(); 

                foreach (var baseFrame in frames)
                {
                    results.Add(new AFEventFrame(baseFrame.ID, baseFrame.Name, baseFrame.Description, baseFrame.Path));
                }

                return results;
            }
        }
    }
}
