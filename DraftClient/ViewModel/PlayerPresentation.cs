namespace DraftClient.ViewModel
{
    using DraftEntities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PlayerPresentation : BindableBase
    {
        private int _adp;
        private string _name;
        private PlayerPosition _position;
        private string _team;
        private int _byeWeek;
        private int _yahooADP;
        private int _eSPNADP;
        private int _cBSADP;
        private decimal _projectedPoints;
        private bool _isPicked;

        public int ADP
        {
            get { return this._adp; }
            set
            {
                SetProperty(ref this._adp, value);
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
        public int YahooADP
        {
            get { return this._yahooADP; }
            set
            {
                SetProperty(ref this._yahooADP, value);
            }
        }
        public int ESPNADP
        {
            get { return this._eSPNADP; }
            set
            {
                SetProperty(ref this._eSPNADP, value);
            }
        }
        public int CBSADP
        {
            get { return this._cBSADP; }
            set
            {
                SetProperty(ref this._cBSADP, value);
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
