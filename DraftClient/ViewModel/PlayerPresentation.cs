namespace DraftClient.ViewModel
{
    using DraftEntities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PlayerPresentation : BindableBase
    {
        private int _averageDraftPosition;
        private string _name;
        private PlayerPosition _position;
        private string _team;
        private int _byeWeek;
        private decimal _projectedPoints;
        private bool _isPicked;

        public int AverageDraftPosition
        {
            get { return this._averageDraftPosition; }
            set
            {
                SetProperty(ref this._averageDraftPosition, value);
            }
        }
        public string Name
        {
            get { return this._name; }
            set
            {
                SetProperty(ref this._name, value);
            }
        }
        public PlayerPosition Position
        {
            get { return this._position; }
            set
            {
                SetProperty(ref this._position, value);
            }
        }
        public string Team
        {
            get { return this._team; }
            set
            {
                SetProperty(ref this._team, value);
            }
        }
        public int ByeWeek
        {
            get { return this._byeWeek; }
            set
            {
                SetProperty(ref this._byeWeek, value);
            }
        }
        public decimal ProjectedPoints
        {
            get { return this._projectedPoints; }
            set
            {
                SetProperty(ref this._projectedPoints, value);
            }
        }
        public bool IsPicked
        {
            get { return this._isPicked; }
            set
            {
                SetProperty(ref this._isPicked, value);
            }
        }
    }
}
