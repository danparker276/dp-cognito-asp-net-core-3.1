﻿using dp.data.AdoNet.DataAccessObjects;
using dp.data.Interfaces;

namespace dp.data.AdoNet
{
    public class DaoFactory : IDaoFactory
    { 
        private string _dpDbConnectionString;
        public DaoFactory(string dpDbConnectionString)
        {
            _dpDbConnectionString = dpDbConnectionString;
        }
        public UserDao UserDao => new UserDao(_dpDbConnectionString);
        public ImageDao ImageDao => new ImageDao(_dpDbConnectionString);
        public TeamDao TeamDao => new TeamDao(_dpDbConnectionString);
    }
    
}