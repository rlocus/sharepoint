using Microsoft.SharePoint.Linq;
using System;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new meeting, deadline or other event.
    /// </summary>
    [ContentType(Name = "Event", Id = "0x0102")]
    public class EntityEvent : EntityItem
    {
        private string _location;

        private DateTime? _startTime;

        private DateTime? _endTime;

        private string _description;

        private bool? _allDayEvent;

        private bool? _recurrence;

        private bool? _workspace;

        private string _category;

        [Column(Name = "Location", Storage = "_location", FieldType = "Text")]
        public string Location
        {
            get
            {
                return this._location;
            }
            set
            {
                if (value == this._location) return;

                this.OnPropertyChanging("Location", this._location);
                this._location = value;
                this.OnPropertyChanged("Location");
            }
        }

        [Column(Name = "StartDate", Storage = "_startTime", Required = true, FieldType = "DateTime")]
        public virtual DateTime? StartTime
        {
            get
            {
                return this._startTime;
            }
            set
            {
                if (value == this._startTime) return;

                this.OnPropertyChanging("StartTime", this._startTime);
                this._startTime = value;
                this.OnPropertyChanged("StartTime");
            }
        }

        [Column(Name = "EndDate", Storage = "_endTime", Required = true, FieldType = "DateTime")]
        public DateTime? EndTime
        {
            get
            {
                return this._endTime;
            }
            set
            {
                if (value == this._endTime) return;

                this.OnPropertyChanging("EndTime", this._endTime);
                this._endTime = value;
                this.OnPropertyChanged("EndTime");
            }
        }

        [Column(Name = "Comments", Storage = "_description", FieldType = "Note")]
        public virtual string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                if (value == this._description) return;

                this.OnPropertyChanging("Description", this._description);
                this._description = value;
                this.OnPropertyChanged("Description");
            }
        }

        [Column(Name = "fAllDayEvent", Storage = "_allDayEvent", FieldType = "AllDayEvent")]
        public bool? AllDayEvent
        {
            get
            {
                return this._allDayEvent;
            }
            set
            {
                if (value == this._allDayEvent) return;

                this.OnPropertyChanging("AllDayEvent", this._allDayEvent);
                this._allDayEvent = value;
                this.OnPropertyChanged("AllDayEvent");
            }
        }

        [Column(Name = "fRecurrence", Storage = "_recurrence", FieldType = "Recurrence")]
        public bool? Recurrence
        {
            get
            {
                return this._recurrence;
            }
            set
            {
                if (value == this._recurrence) return;

                this.OnPropertyChanging("Recurrence", this._recurrence);
                this._recurrence = value;
                this.OnPropertyChanged("Recurrence");
            }
        }

        [Column(Name = "WorkspaceLink", Storage = "_workspace", FieldType = "CrossProjectLink")]
        public bool? Workspace
        {
            get
            {
                return this._workspace;
            }
            set
            {
                if (value == this._workspace) return;

                this.OnPropertyChanging("Workspace", this._workspace);
                this._workspace = value;
                this.OnPropertyChanged("Workspace");
            }
        }

        [Column(Name = "Category", Storage = "_category", FieldType = "Choice")]
        public string Category
        {
            get
            {
                return this._category;
            }
            set
            {
                if (value == this._category) return;

                this.OnPropertyChanging("Category", this._category);
                this._category = value;
                this.OnPropertyChanged("Category");
            }
        }
    }
}
