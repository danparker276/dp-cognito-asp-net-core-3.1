using System;
using System.Collections.Generic;
using System.Text;

namespace dp.business.Enums
{

    public enum DataProvider
    {
        Npgsql,
        AdoNet
    }

    public enum UserType
    {
        Anon =0,
        User = 1,
        Admin = 2,
    }
    public enum UserStatus
    {
        New = 0,
        Acitve = 1,
        Inactive = 2
    }
    public enum TeamSort
    {
        Credits = 0,
        Updated = 1,
        TeamName = 2
    }
    public enum Sort
    {
        Desc = 0,
        Asc = 1,
    }
    public enum KeyActiveState
    {
        Inactive = 0,
        Active = 1,
        Deleted = 2
    }
    public enum TeamExpand
    {
        Flat = 0,
        Info = 1,
        Docs = 1,
        Images = 2
    }
}
