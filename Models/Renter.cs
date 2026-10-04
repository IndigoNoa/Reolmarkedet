using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.Models
{
    public class Renter(string name) : ViewModelBase
    {
        private int id;
        private string name = name;
        private string? address;
        private string? phone;
        private string? email;
        private int desiredShelfCount;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
    }
}

        public string Name
    {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Address
        {
            get { return address; }
            set
        {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
            }

        public string Phone
    {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public int DesiredShelfCount
        {
            get { return desiredShelfCount; }
            set
            {
                desiredShelfCount = value;
                OnPropertyChanged(nameof(DesiredShelfCount));
            }
            }

        public event PropertyChangedEventHandler PropertyChanged;

        protected new virtual void OnPropertyChanged(string propertyName)
    {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    }
}
}
