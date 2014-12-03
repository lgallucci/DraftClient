namespace DraftClient.ViewModel
{
    using System;
    using System.IO.Packaging;
    using System.Runtime.CompilerServices;

    public abstract class ValidatableBase : BindableBase
    {
        private bool _isValid;
        public bool IsValid
        {
            get { return this._isValid; }
            set
            {
                base.SetProperty<bool>(ref this._isValid, value);
            }
        }

        protected new bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            bool ret = base.SetProperty(ref storage, value, propertyName);
            IsValid = Validate();
            return ret;
        }

        public abstract bool Validate();
    }
}
