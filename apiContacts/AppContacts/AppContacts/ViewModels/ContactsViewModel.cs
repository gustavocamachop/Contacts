﻿using AppContacts.Models;
using AppContacts.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AppContacts.ViewModels
{
    public class ContactsViewModel: BaseViewModel
    {
        #region Attributes
        private ApiService apiService;
        private ObservableCollection<Contacts> contacts;
        #endregion

        #region Properties
        public ObservableCollection<Contacts> Contacts
        {
            get { return this.contacts; }
            set { SetValue(ref this.contacts, value);}
        }
        #endregion
        #region Constructor
        public ContactsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadContacts();
        }

        #endregion
        #region Methods
        private async void LoadContacts()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Internet Error Connection",
                    connection.Message,
                    "Accept"
                    );
                return;
            }
            var response = await apiService.GetList<Contacts>(
                "http://localhost:49259/",
                "api/",
                "Contacts"
                );
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Contacts Service Error",
                    response.Message,
                    "Accept"
                    );
                return;
            }
            MainViewModel main = MainViewModel.GetInstance();
            main.ListContacts = (List<Contacts>)response.Result;
            this.Contacts = new ObservableCollection<Contacts>(ToContactsCollect());
        }

        private IEnumerable<Contacts> ToContactsCollect()
        {
            ObservableCollection<Contacts> collection = new ObservableCollection<Contacts>();
            MainViewModel main = MainViewModel.GetInstance();
            foreach (var lista in main.ListContacts)
            {
                Contacts contact = new Contacts();
                contact.ID = lista.ID;
                contact.Name = lista.Name;
                contact.Phone = lista.Phone;
                contact.Bussines = lista.Bussines;
                collection.Add(contact);
            }
            return (collection);
        }
        #endregion
    }
}