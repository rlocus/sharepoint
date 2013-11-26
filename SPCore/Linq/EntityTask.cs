using Microsoft.SharePoint.Linq;
using System;

namespace SPCore.Linq
{
    /// <summary>
    /// Track a work item that you or your team needs to complete.
    /// </summary>
    [ContentType(Name = "Task", Id = "0x0108")]
    public class EntityTask : EntityItem
    {
        private double? _complete;

        private string _body;

        private DateTime? _startDate;

        private DateTime? _dueDate;

        private Priority? _priority;

        private TaskStatus? _taskStatus;

        private int? _assignedToId;

        private string _assignedTo;

        [Column(Name = "PercentComplete", Storage = "_complete", FieldType = "Number")]
        public double? Complete
        {
            get
            {
                return this._complete;
            }
            set
            {
                if (value == this._complete) return;

                this.OnPropertyChanging("Complete", this._complete);
                this._complete = value;
                this.OnPropertyChanged("Complete");
            }
        }

        [Column(Name = "Body", Storage = "_body", FieldType = "Note")]
        public string Body
        {
            get
            {
                return this._body;
            }
            set
            {
                if (value == this._body) return;

                this.OnPropertyChanging("Body", this._body);
                this._body = value;
                this.OnPropertyChanged("Body");
            }
        }

        [Column(Name = "StartDate", Storage = "_startDate", FieldType = "DateTime")]
        public DateTime? StartDate
        {
            get
            {
                return this._startDate;
            }
            set
            {
                if (value == this._startDate) return;

                this.OnPropertyChanging("StartDate", this._startDate);
                this._startDate = value;
                this.OnPropertyChanged("StartDate");
            }
        }

        [Column(Name = "TaskDueDate", Storage = "_dueDate", FieldType = "DateTime")]
        public virtual DateTime? DueDate
        {
            get
            {
                return this._dueDate;
            }
            set
            {
                if (value == this._dueDate) return;

                this.OnPropertyChanging("DueDate", this._dueDate);
                this._dueDate = value;
                this.OnPropertyChanged("DueDate");
            }
        }

        [Column(Name = "Priority", Storage = "_priority", FieldType = "Choice")]
        public Priority? Priority
        {
            get
            {
                return this._priority;
            }
            set
            {
                if (value == this._priority) return;

                this.OnPropertyChanging("Priority", this._priority);
                this._priority = value;
                this.OnPropertyChanged("Priority");
            }
        }

        [Column(Name = "TaskStatus", Storage = "_taskStatus", FieldType = "Choice")]
        public virtual TaskStatus? TaskStatus
        {
            get
            {
                return this._taskStatus;
            }
            set
            {
                if (value == this._taskStatus) return;

                this.OnPropertyChanging("TaskStatus", this._taskStatus);
                this._taskStatus = value;
                this.OnPropertyChanged("TaskStatus");
            }
        }

        [Column(Name = "AssignedTo", Storage = "_assignedToId", FieldType = "User", IsLookupId = true)]
        public int? AssignedToId
        {
            get
            {
                return this._assignedToId;
            }
            set
            {
                if (value == this._assignedToId) return;

                this.OnPropertyChanging("AssignedToId", this._assignedToId);
                this._assignedToId = value;
                this.OnPropertyChanged("AssignedToId");
            }
        }

        [Column(Name = "AssignedTo", Storage = "_assignedTo", ReadOnly = true, FieldType = "User", IsLookupValue = true)]
        public string AssignedTo
        {
            get
            {
                return this._assignedTo;
            }
            set
            {
                if (value == this._assignedTo) return;

                this.OnPropertyChanging("AssignedTo", this._assignedTo);
                this._assignedTo = value;
                this.OnPropertyChanged("AssignedTo");
            }
        }
    }

    public enum Priority
    {
        None = 0,

        Invalid = 1,

        [Choice(Value = "(1) High")]
        High = 2,

        [Choice(Value = "(2) Normal")]
        Normal = 4,

        [Choice(Value = "(3) Low")]
        Low = 8,
    }

    public enum TaskStatus
    {
        None = 0,

        Invalid = 1,

        [Choice(Value = "Not Started")]
        NotStarted = 2,

        [Choice(Value = "In Progress")]
        InProgress = 4,

        [Choice(Value = "Completed")]
        Completed = 8,

        [Choice(Value = "Deferred")]
        Deferred = 16,

        [Choice(Value = "Waiting on someone else")]
        WaitingOnSomeoneElse = 32,
    }
}
