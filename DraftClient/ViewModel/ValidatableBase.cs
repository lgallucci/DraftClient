namespace DraftClient.ViewModel
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class ValidatableBase : BindableBase
    {
        private bool _isValid;

        public bool IsValid
        {
            get { return _isValid; }
            set { base.SetProperty(ref _isValid, value); }
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